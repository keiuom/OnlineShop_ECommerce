using Serilog;

namespace BuyNow.API.StartupServices
{
    public static class LoggingExtensions
    {
        public static void AddSerilog(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
            hostBuilder.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));
        }
    }
}
