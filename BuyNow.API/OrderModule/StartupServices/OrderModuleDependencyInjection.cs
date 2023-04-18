﻿using Order.Data;
using Order.Services;

namespace BuyNow.API.OrderModule.StartupServices
{
    public static class OrderModuleDependencyInjection
    {
        public static void RegisterOrderModuleDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.LoadDependency();
        }
    }
}
