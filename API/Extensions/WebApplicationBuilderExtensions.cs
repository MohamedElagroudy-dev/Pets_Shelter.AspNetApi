
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

                var redisPassword = builder.Configuration["ConnectionStrings:RedisPassword"];
                var redisEndpoint = builder.Configuration["ConnectionStrings:RedisEndpoint"];
                if (redisEndpoint is null) // just for null warning
                    redisEndpoint = "";

                // Important for Upstash (TLS/SSL required)
                config.Ssl = true;
                config.Password = redisPassword;
                config.EndPoints.Add(redisEndpoint);

                return ConnectionMultiplexer.Connect(config);
            });

            builder.Services.AddSignalR();
        }
    }
}
