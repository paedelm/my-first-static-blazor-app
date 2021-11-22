using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
const string localApiBase = "http://localhost:7071/";
var baseAddress = builder.HostEnvironment.BaseAddress ?? localApiBase;
if (baseAddress.ToLower().Contains("localhost"))
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(localApiBase) });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
}

// http://localhost:7071/api/WeatherForecast
// swa start http://localhost:5000 --run "dotnet run --project Client/Client.csproj" --api-location Api
await builder.Build().RunAsync();
