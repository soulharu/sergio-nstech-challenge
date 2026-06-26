using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Common;
using Orders.Application.Services;
using Orders.Domain.Interfaces.Services;

namespace Orders.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
