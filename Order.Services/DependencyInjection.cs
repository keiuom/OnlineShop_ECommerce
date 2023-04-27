﻿using Microsoft.Extensions.DependencyInjection;
using Order.Services.HttpClients;
using Order.Services.Mails;
using Order.Services.Orders;

namespace OrderModule.Services
{
    public static class DependencyInjection
    {
        public static void LoadDependency(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<IOrderClient, OrderClient>();
        }
    }
}
