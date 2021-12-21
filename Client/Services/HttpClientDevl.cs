using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BlazorApp.Shared;
using EnvironmentNS;
using BlazorApp.Client.AuthProviders;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Client.Services
{
    public class HttpClientDevl
    {
        public HttpClientDevl(HttpClient http, AuthenticationStateProvider authProvider)
        {
            Http = http;
            AuthProvider = authProvider;
        }

        //private static readonly ClientPrincipal paedelm = new() { IdentityProvider = "github", UserId = "33", UserDetails = "paedelm", UserRoles = new string[] { "authenticated" } };
        private HttpClient Http { get; }
        private AuthenticationStateProvider AuthProvider { get; }
        public async Task<T?> GetFromJsonAsync<T>(string apicall)
        {
            Func<ClientPrincipal, string> ToHeader = (ClientPrincipal clientPrincipal) =>
            {

                var data = JsonSerializer.Serialize(clientPrincipal);
                Console.WriteLine($"clientPrincipal={data}");
                UTF8Encoding encUtf8 = new();
                return Convert.ToBase64String(encUtf8.GetBytes(data));
            };

            var request = new HttpRequestMessage(HttpMethod.Get, apicall);

            // add authorization header
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "my-token");

            // add custom http header
            if (Env.EnvName ==  "Development")
            {
                var authProvider = AuthProvider as TestAuthStateProvider;
                if (authProvider != null) request.Headers.Add("x-ms-client-principal", ToHeader(authProvider.ClientPrincipal));
            }

            // send request
            using var httpResponse = await Http.SendAsync(request);

            // convert http response data to UsersResponse object
            var response = await httpResponse.Content.ReadFromJsonAsync<T>();
            return response;
        }
    }

}
