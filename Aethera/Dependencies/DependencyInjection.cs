using Aethera.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using Aethera.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

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

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }


        public static IServiceCollection AddAuthenticationWithJwt(this IServiceCollection services, IConfiguration config)
        {
            var authority = config["AzureEntraExternalID:Authority"];
            var clientId = config["AzureEntraExternalID:ClientId"];
            var audience = config["AzureEntraExternalID:Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    //options.Audience = audience;

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authority,
                        ValidateAudience = true,
                        ValidAudiences = new[]
                {
                    audience, 
                    $"api://{audience}" 
                },
                        ValidateLifetime = true
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}