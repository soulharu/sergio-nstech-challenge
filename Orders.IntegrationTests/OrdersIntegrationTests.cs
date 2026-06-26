using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Orders.Application.DTOs;
using Orders.Domain.Enums;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Orders.IntegrationTests
{
    public class OrdersIntegrationTests : IClassFixture<OrderServiceWebFactory>
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OrdersIntegrationTests(OrderServiceWebFactory factory)
        {
            factory.SeedDatabase();
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5001/"),
                AllowAutoRedirect = false
            });
        }

        private async Task AuthenticateAsync_Admin()
        {
            var response = await _client.PostAsJsonAsync("/auth/token", new { username = "admin", password = "admin123" });
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = content.GetProperty("token").GetString()!;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task AuthenticateAsync_User()
        {
            var response = await _client.PostAsJsonAsync("/auth/token", new { username = "user", password = "user123" });
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = content.GetProperty("token").GetString()!;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task PostToken_WithValidCredentials_ShouldReturnJwt()
        {
            //Arrange & Act 
            var response = await _client.PostAsJsonAsync("/auth/token", new { username = "user", password = "user123" });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            body.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task PostToken_WithInvalidCredentials_ShouldReturn401()
        {
            //Arrange & Act 
            var response = await _client.PostAsJsonAsync("/auth/token", new { username = "wrong", password = "wrong" });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetOrders_WithoutToken_ShouldReturn401()
        {
            //Arrange & Act 
            var response = await _client.GetAsync("/orders");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateOrder_WithValidPayload_ShouldReturn201WithPlacedStatus()
        {
            //Arrange
            await AuthenticateAsync_User();

            var payload = new
            {
                customerId = Guid.NewGuid(),
                currency = "BRL",
                items = new[] { new { productId = OrderServiceWebFactory.Product1Id, quantity = 2 } }
            };

            //Act 
            var response = await _client.PostAsJsonAsync("/orders", payload);


            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var order = await response.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
            order!.Status.Should().Be(OrderStatus.Placed.ToString());
            order.Total.Should().Be(58.40m);
        }

        [Fact]
        public async Task CreateOrder_WithNoItems_ShouldReturn400()
        {
            //Arrange 
            await AuthenticateAsync_User();

            var payload = new { customerId = Guid.NewGuid(), currency = "BRL", items = Array.Empty<object>() };

            //Act 
            var response = await _client.PostAsJsonAsync("/orders", payload);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ConfirmOrder_ShouldTransitionToConfirmed()
        {
            //Arrange
            await AuthenticateAsync_Admin();

            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "USD",
                items = new[] { new { productId = OrderServiceWebFactory.Product2Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            //Act 
            var confirmResponse = await _client.PostAsync($"/orders/{order!.Id}/confirm", null);

            //Assert
            confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var confirmed = await confirmResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
            confirmed!.Status.Should().Be(OrderStatus.Confirmed.ToString());
        }

        [Fact]
        public async Task ConfirmOrder_CalledTwice_ShouldBeIdempotent()
        {
            //Arrange
            await AuthenticateAsync_Admin();

            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "BRL",
                items = new[] { new { productId = OrderServiceWebFactory.Product1Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            await _client.PostAsync($"/orders/{order!.Id}/confirm", null);

            //Act 
            var secondConfirm = await _client.PostAsync($"/orders/{order.Id}/confirm", null);

            //Assert 
            secondConfirm.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CancelOrder_WhenPlaced_ShouldTransitionToCanceled()
        {
            //Arrange
            await AuthenticateAsync_Admin();

            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "BRL",
                items = new[] { new { productId = OrderServiceWebFactory.Product1Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            //Act 
            var cancelResponse = await _client.PostAsync($"/orders/{order!.Id}/cancel", null);

            //Assert 
            cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var canceled = await cancelResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
            canceled!.Status.Should().Be(OrderStatus.Canceled.ToString());
        }

        [Fact]
        public async Task GetOrder_WithValidId_ShouldReturnOrder()
        {
            //Arrange
            await AuthenticateAsync_User();

            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "BRL",
                items = new[] { new { productId = OrderServiceWebFactory.Product1Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var created = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            //Act 
            var getResponse = await _client.GetAsync($"/orders/{created!.Id}");

            //Assert 
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
            fetched!.Id.Should().Be(created.Id);
            fetched.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetOrder_WithInvalidId_ShouldReturn404()
        {
            //Arrange
            await AuthenticateAsync_User();

            //Act 
            var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ListOrders_ShouldReturnPagedResult()
        {
            //Arrange 
            await AuthenticateAsync_User();

            //Act 
            var response = await _client.GetAsync("/orders?page=1&pageSize=10");

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ConfirmOrderAsUser_ShouldReturn403()
        {
            //Arrange
            await AuthenticateAsync_User();
          
            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "USD",
                items = new[] { new { productId = OrderServiceWebFactory.Product2Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            //Act 
            var confirmResponse = await _client.PostAsync($"/orders/{order!.Id}/confirm", null);

            //Assert
            confirmResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CancelOrderAsUser_ShouldReturn403()
        {
            //Arrange
            await AuthenticateAsync_User();

            var createPayload = new
            {
                customerId = Guid.NewGuid(),
                currency = "BRL",
                items = new[] { new { productId = OrderServiceWebFactory.Product1Id, quantity = 1 } }
            };
            var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
            var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

            //Act 
            var cancelResponse = await _client.PostAsync($"/orders/{order!.Id}/cancel", null);

            //Assert 
            cancelResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }
}
