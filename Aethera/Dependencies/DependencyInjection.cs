using Aethera.Services;
using Aethera.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using Aethera.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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
                
                var logger = s.GetRequiredService<ILogger<CosmosClient>>();

                
                if (string.IsNullOrEmpty(options.AccountEndpoint))
                {
                    logger.LogError("CosmosDbSettings:AccountEndpoint is null or empty.");
                    throw new InvalidOperationException("CosmosDbSettings:AccountEndpoint is missing. Please check the secret 'CosmosDbSettings--AccountEndpoint' in Azure Key Vault.");
                }

                if (string.IsNullOrEmpty(options.AccountKey))
                {
                    logger.LogError("CosmosDbSettings:AccountKey is null or empty.");
                    
                    throw new InvalidOperationException("CosmosDbSettings:AccountKey (authKeyOrResourceToken) is missing. Please check the secret 'CosmosDbSettings--AccountKey' in Azure Key Vault.");
                }

                logger.LogInformation($" Connecting to Cosmos DB: {options.AccountEndpoint} (Database: {options.DatabaseName})");

                

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

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IEmailService, SendGridEmailService>();

            return services;
        }


        public static IServiceCollection AddAuthenticationWithJwt(this IServiceCollection services, IConfiguration config)
        {
            var authority = config["AzureEntraExternalID-Authority"];
            var clientId = config["AzureEntraExternalID-ClientId"];
            var audience = config["AzureEntraExternalID-Audience"];

            
            var serviceProvider = services.BuildServiceProvider();

           
            var logger = serviceProvider.GetService<ILogger<Program>>();
            //logger?.LogInformation($"Authentication Configuration:");
            //logger?.LogInformation($"  Authority: {authority}");
            //logger?.LogInformation($"  ClientId: {clientId}");
            //logger?.LogInformation($"  Audience: {audience}");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                   
                    options.Authority = authority;

                   

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudiences = new[]
                        {
                            audience,
                            clientId  
                            //$"api://{audience}"
                        },
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(5)

                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
            });

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services, IConfiguration config)
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

                var scope = config["AzureEntraExternalID-SwaggerScope"];

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
                        new string[] { scope }
                    }
                });
            });

            return services;
        }
    }
}


