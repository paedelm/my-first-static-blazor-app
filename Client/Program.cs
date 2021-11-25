using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using BlazorApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp.Client.AuthProviders;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
const string localApiBase = "http://localhost:7071/";
EnvironmentNS.Env.EnvName = builder.HostEnvironment.Environment;
EnvironmentNS.Env.HostEnv = builder.HostEnvironment;
Console.WriteLine($"environment={builder.HostEnvironment.Environment}");
var baseAddress = builder.HostEnvironment.BaseAddress ?? localApiBase;
if (baseAddress.ToLower().Contains("localhost"))
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(localApiBase) })
    .AddAuthorizationCore()
    .AddScoped<AuthenticationStateProvider, TestAuthStateProvider>();

}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
}
builder.Services.AddScoped(_ => new MyEnvironment(hostEnvironment: builder.HostEnvironment));
// http://localhost:7071/api/WeatherForecast
// swa start http://localhost:5000 --run "dotnet run --project Client/Client.csproj" --api-location Api
await builder.Build().RunAsync();
namespace EnvironmentNS
{
    public class Env
    {
        private static IWebAssemblyHostEnvironment? _hostenv;
        public static IWebAssemblyHostEnvironment? HostEnv { get => _hostenv; set => _hostenv = value; }
        private static string _env = "Development";
        public static string EnvName { get => _env; set => _env = value; }
    }
}