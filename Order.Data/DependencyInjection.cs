using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Data.DbContexts;
using Order.Data.Repositories;
using Order.Data.RepositoryWrappers;
using OrderModule.Data.DbContexts;
using OrderModule.Data.Repositories;

namespace OrderModule.Data
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

            services.AddDbContext<EmailDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(EmailDbContext).Assembly.FullName)));

            Load(services);
        }

        private static void Load(IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IQueueMailRepositoryWrapper, QueueMailRepositoryWrapper>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
        }
    }
}
