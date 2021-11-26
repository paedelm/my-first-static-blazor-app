using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EnvironmentNS;
namespace BlazorApp.Client.AuthProviders
{
    public class TestAuthStateProvider : AuthenticationStateProvider
    {
        private HttpClient _httpClient;
        public TestAuthStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(1500);
            List<Claim> claims = ClaimList("Peter");
            var anonymous = new ClaimsIdentity();
            var peter = new ClaimsIdentity(claims, "TestAuthType");
            if (Env.EnvName == "Development")
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(peter)));
            }
            else
            {
                var clientPrincipal = (await _httpClient.GetFromJsonAsync<ClientPrincipal>("/.auth/me"));
                if (clientPrincipal != null)
                {

                    var ident = new ClaimsIdentity(ClaimList(clientPrincipal.UserDetails), clientPrincipal.IdentityProvider);
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(ident)));
                } else
                {
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
                }
            }
        }

        private static List<Claim> ClaimList(string name)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "lezer"),
                new Claim(ClaimTypes.Role, "anonymous"),
                new Claim(ClaimTypes.Role, "authenticated"),
            };
        }
    }
    class ClientPrincipal
    {
        public string? IdentityProvider { get; set; }
        public string? UserId { get; set; }
        public string? UserDetails { get; set; }
        public string[]? UserRoles { get; set; }

    }
}
