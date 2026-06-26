using Mediator;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Contracts.Order;
using Orders.Application.Commands.CancelOrder;
using Orders.Application.Commands.ConfirmOrder;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.Commands.CreateOrderItem;
using Orders.Application.Common;
using Orders.Application.DTOs;
using Orders.Application.Queries.GetOrderById;
using Orders.Application.Queries.ListOrders;
using Orders.Domain.Enums;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request,CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand(
                request.CustomerId,
                request.Currency,
                request.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)));

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListOrders(
        [FromQuery] Guid? customerId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new ListOrdersQuery(customerId, status, from, to, page, pageSize),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{id:guid}/confirm")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ConfirmOrder(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ConfirmOrderCommand(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
            return Ok(result);
        }
    }
}
