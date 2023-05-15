using CarManager.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace CarManager.Utils.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

        builder.Services.AddSingleton<CosmosClient>(_ =>
        {
            var connectionString = builder.Configuration.GetConnectionString("CosmosAccount") ??
                                   throw new InvalidOperationException("\"CosmosAccount\" connection string is not provided.");
            
            var cosmosClientBuilder = new CosmosClientBuilder(connectionString).WithSerializerOptions(
                new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase });

            return cosmosClientBuilder.Build();
        });

        builder.Services.AddSingleton<CosmosAccountClient>();

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


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");
    }
}