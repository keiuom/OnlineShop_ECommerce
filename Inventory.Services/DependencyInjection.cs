using Inventory.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Services
{
    public static class DependencyInjection
    {
        public static void LoadDependency(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
        }
    }
}
