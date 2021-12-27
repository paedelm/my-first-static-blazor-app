using System.Text;
using System.Text.Json;
using EnvironmentNS;
using BlazorApp.Shared;
using BlazorApp.Client.AuthProviders;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Client.Shared
{
    class MessageHandlerDevl : DelegatingHandler
    {
        public MessageHandlerDevl(AuthenticationStateProvider authProvider)
        {
            AuthProvider = authProvider;
        }
        public AuthenticationStateProvider AuthProvider { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (Env.EnvName == "Development")
            {
                var authProvider = AuthProvider as TestAuthStateProvider;
                if (authProvider != null) request.Headers.Add("x-ms-client-principal", ToHeader(authProvider.ClientPrincipal));

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

