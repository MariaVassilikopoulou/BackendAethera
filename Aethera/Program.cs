using Aethera.Dependencies;
using Aethera.MappingProfiles;
using Aethera.Repositories;
using Azure.Identity;
using Microsoft.Azure.Cosmos;


var builder = WebApplication.CreateBuilder(args);


var keyVaultUri = builder.Configuration["KeyVault:Uri"];

if (!string.IsNullOrEmpty(keyVaultUri))
{
    Console.WriteLine($"Configuring Azure Key Vault from URI: {keyVaultUri}");
    try
    {
        // Add Azure Key Vault as a configuration source using DefaultAzureCredential.
        // This is the most robust way to authenticate, as it automatically uses:
        // 1. Managed Identity (when deployed to Azure App Service, Function, etc.)
        // 2. Local Azure CLI, Visual Studio, or VS Code credentials (when running locally)
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());

        Console.WriteLine("Successfully loaded configuration from Azure Key Vault.");
    }
    catch (Exception ex)
    {
        // It's good practice to log a warning and let the application start with other config sources.
        Console.WriteLine($"\n--- WARNING: Failed to load configuration from Azure Key Vault ---\n{ex.Message}\n");
        Console.WriteLine("Check: KeyVault URI is correct, permissions are granted to the executing identity, and you have run 'az login' locally.");
        // We do not re-throw, allowing the application to proceed using existing appsettings.json/environment variables.
    }
}
else
{
    Console.WriteLine("INFO: Azure Key Vault URI is not configured (missing 'KeyVault:Uri'). Skipping Key Vault loading.");
}


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationWithJwt(builder.Configuration);
builder.Services.AddSwaggerWithJwt(builder.Configuration);
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddApplicationServices();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000",
                         "https://aethera-eight.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
var app = builder.Build();

// CORS must be first so headers are present even on error responses
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Allow Stripe webhook to read the raw request body for signature verification
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/payment/webhook"))
        context.Request.EnableBuffering();
    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();