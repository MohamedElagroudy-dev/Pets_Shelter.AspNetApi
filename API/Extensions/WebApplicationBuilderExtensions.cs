
using StackExchange.Redis;

namespace E_commerce.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddPresentation(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json", optional: false)
                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connString = builder.Configuration.GetConnectionString("Redis")
                    ?? throw new Exception("Cannot get Redis connection string");

                var config = ConfigurationOptions.Parse(connString);
                config.AbortOnConnectFail = false;

                // Important for Upstash (TLS/SSL required)
                config.Ssl = true;
                config.Password = "AUy5AAIncDFmYmU2MTcxNDI0ZGQ0OTFlOWVjZjcyMzA1YzhiOGQzYXAxMTk2NDE";
                config.EndPoints.Add("unbiased-teal-19641.upstash.io:6379");

                return ConnectionMultiplexer.Connect(config);
            });

            builder.Services.AddSignalR();
        }
    }
}
