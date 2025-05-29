using Aethera.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Aethera.Dependencies
{
    public static class DependencyInjection
    {


        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CosmosDbSettings>(config.GetSection("CosmosDbSettings"));

            services.AddSingleton(s =>
            {
                var options = s.GetRequiredService<IOptions<CosmosDbSettings>>().Value;

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var clientOptions = new CosmosClientOptions
                {
                    Serializer = new SystemTextJsonCosmosSerializer(jsonOptions),
                };

                return new CosmosClient(options.AccountEndpoint, options.AccountKey, clientOptions);
            });

            return services;
        }



    }
}
