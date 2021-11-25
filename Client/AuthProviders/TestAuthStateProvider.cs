using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Peter"),
                new Claim(ClaimTypes.Role, "lezer"),
                new Claim(ClaimTypes.Role, "anonymous"),
                new Claim(ClaimTypes.Role, "authenticated"),
            };
            var anonymous = new ClaimsIdentity();
            var peter = new ClaimsIdentity(claims);
            var x = await _httpClient.GetFromJsonAsync<AuthenticationState>("/.auth/me") ?? new AuthenticationState(new ClaimsPrincipal(peter));
            return await Task.FromResult(x);
        }
    }
}
