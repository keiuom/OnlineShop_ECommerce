using Microsoft.Extensions.DependencyInjection;
using Order.Services.Orders;

namespace OrderModule.Services
{
    public static class DependencyInjection
    {
        public static void LoadDependency(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
