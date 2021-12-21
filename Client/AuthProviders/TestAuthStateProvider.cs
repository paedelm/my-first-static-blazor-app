using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EnvironmentNS;
using BlazorApp.Client.Services;
using BlazorApp.Shared;

namespace BlazorApp.Client.AuthProviders
{
    public class TestAuthStateProvider : AuthenticationStateProvider
    {
        private HttpClient _httpClient;
        private readonly MyEnvironment myEnvironment;
        public ClientPrincipal ClientPrincipal { get; set; } = new ClientPrincipal();

        public TestAuthStateProvider(HttpClient httpClient, MyEnvironment myEnvironment)
        {
            _httpClient = httpClient;
            this.myEnvironment=myEnvironment;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(1500);
            List<Claim> claims = ClaimList("Peter");
            var anonymous = new ClaimsIdentity();
            Func<PrincipalRec?, Task<AuthenticationState>> genState = async (PrincipalRec? rec) =>
            {
                if (rec?.clientPrincipal != null)
                {

                    var ident = new ClaimsIdentity(ClaimList(rec.clientPrincipal?.UserDetails ?? "Peter Def"), rec.clientPrincipal?.IdentityProvider ?? "GithubDef");
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(ident)));
                }
                else
                {
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
                }

            };
            var peter = new ClaimsIdentity(claims, "TestAuthType");
            if (Env.EnvName == "Development")
            {
                //var http = new HttpClient { BaseAddress = new Uri(myEnvironment.HostEnvironment.BaseAddress) };
                var rec = (await _httpClient.GetFromJsonAsync<PrincipalRec>($"{myEnvironment.HostEnvironment.BaseAddress}me.json"));
                Console.WriteLine($"-- clientprincipal = {rec?.clientPrincipal?.IdentityProvider}, {rec?.clientPrincipal?.UserDetails}");
                //return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(peter)));
                ClientPrincipal = rec?.clientPrincipal ?? new ClientPrincipal();
                return await genState(rec);
            }
            else
            {
                var rec = (await _httpClient.GetFromJsonAsync<PrincipalRec>("/.auth/me"));
                ClientPrincipal = rec?.clientPrincipal ?? new ClientPrincipal(); ;
                return await genState(rec);
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
    //public class ClientPrincipal
    //{
    //    public string? identityProvider { get; set; }
    //    public string? userId { get; set; }
    //    public string? userDetails { get; set; }
    //    public string[]? userRoles { get; set; }

    //}
    record PrincipalRec(ClientPrincipal clientPrincipal);
}
