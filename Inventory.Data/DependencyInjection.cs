using Inventory.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Data
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("InventoryConnection"),
                    b => b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));
        }
    }
}
