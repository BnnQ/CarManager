using System.Text.Json;
using CarManager.Models;
using CarManager.Services;
using CarManager.Services.Abstractions;
using CarManager.Services.MapperProfiles;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace CarManager.Utils.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<Configuration.Azure>(builder.Configuration.GetRequiredSection("Azure"));

        builder.Services.AddControllersWithViews();

        builder.Services.AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions
            { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        builder.Services.AddSingleton<CosmosSerializationOptions>(_ => new CosmosSerializationOptions
            { Indented = false, PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase });

        builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            var connectionString = builder.Configuration.GetConnectionString("CosmosAccount") ??
                                   throw new InvalidOperationException("'CosmosAccount' connection string is not provided.");

            var serializerOptions = serviceProvider.GetRequiredService<CosmosSerializationOptions>();
            var cosmosClientBuilder = new CosmosClientBuilder(connectionString).WithSerializerOptions(serializerOptions);

            return cosmosClientBuilder.Build();
        });

        builder.Services.AddTransient<IIdentifierGenerator, GuidIdentifierGenerator>();
        builder.Services.AddSingleton<CosmosAccountClient>();
        builder.Services.AddTransient<IRepository<Car, string>, CosmosCarRepository>();

        builder.Services.AddAutoMapper(profiles => { profiles.AddProfile<CarMapperProfile>(); });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.WithOrigins("https://localhost:44495")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });


        return builder;
    }

    public static void Configure(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors();

        app.MapControllers();

        app.MapFallbackToFile("index.html");
    }
}