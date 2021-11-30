using System.Text;
using System.Text.Json;
using EnvironmentNS;
using BlazorApp.Shared;
namespace BlazorApp.Client.Shared
{
    class MessageHandlerDevl : DelegatingHandler
    {
        private static readonly ClientPrincipal paedelm = new() { IdentityProvider = "github", UserId = "33", UserDetails = "paedelm", UserRoles = new string[] { "authenticated" } };

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (Env.EnvName == "Development")
            {
                request.Headers.Add("x-ms-client-principal", ToHeader(paedelm));
            }
            return base.SendAsync(request, cancellationToken);
        }
        private static string ToHeader(ClientPrincipal principal)
        {
            var data = JsonSerializer.Serialize(principal);
            UTF8Encoding encUtf8 = new();
            return Convert.ToBase64String(encUtf8.GetBytes(data));
        }
    }
}

