using Order.Core.Settings;
using OrderModule.Data;
using OrderModule.Services;

namespace BuyNow.API.OrderModule.StartupServices
{
    public static class OrderModuleDependencyInjection
    {
        public static void RegisterOrderModuleDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.LoadDependency();

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        }
    }
}
