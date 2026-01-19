using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StoreManager.Webapp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register HttpClient with API base address
if (builder.HostEnvironment.IsDevelopment()) // Check if development or production environment
{
    builder.Services.AddScoped(sp => new HttpClient
    {
        //BaseAddress = new Uri("http://localhost:5088")
        BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5088")
        //BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:7157")
    });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri("https://api.yourproductionurl.com") // Your production API URL
    });
}

builder.Services.AddStoreManagerServices();

await builder.Build().RunAsync();
