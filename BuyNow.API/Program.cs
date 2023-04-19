using BuyNow.API.Inventory.StartupServices;
using BuyNow.API.OrderModule.StartupServices;
using BuyNow.API.StartupServices;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterInventoryDependencyInjection(builder.Configuration);
builder.Services.RegisterOrderModuleDependencyInjection(builder.Configuration);

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
