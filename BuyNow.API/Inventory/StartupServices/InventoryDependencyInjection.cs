using Inventory.Data;
using Inventory.Services;

namespace BuyNow.API.Inventory.StartupServices
{
    public static class InventoryDependencyInjection
    {
        public static void RegisterInventoryDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.LoadDependency();
        }
    }
}
