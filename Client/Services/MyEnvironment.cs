using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorApp.Client.Services
{
    public class MyEnvironment
    {
        public MyEnvironment(IWebAssemblyHostEnvironment hostEnvironment)
        {
            HostEnvironment=hostEnvironment;
        }

        public IWebAssemblyHostEnvironment HostEnvironment { get; }
    }
}
