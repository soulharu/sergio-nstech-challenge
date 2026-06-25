using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Services;
using Orders.Domain.Interfaces.Services;

namespace Orders.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
