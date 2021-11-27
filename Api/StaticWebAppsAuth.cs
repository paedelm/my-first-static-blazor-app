using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;
#nullable enable
namespace BlazorApp.Api
{
    public static class StaticWebAppsAuth
    {
        private class ClientPrincipal
        {
            public string? IdentityProvider { get; init; }
            public string? UserId { get; init; }
            public string? UserDetails { get; init; }
            public IEnumerable<string>? UserRoles { get; set; }
        }

        public static ClaimsPrincipal Parse(HttpRequest req)
        {
            bool hasPrincipalHeader = req.Headers.TryGetValue("x-ms-client-principal", out var header);
            if (!hasPrincipalHeader) return new ClaimsPrincipal();
            Func<ClientPrincipal> fromHeader = () =>
            {
                var data = header[0];
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                return JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            };
            ClientPrincipal principal = fromHeader();
            principal.UserRoles = principal.UserRoles?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

            if ((!principal.UserRoles?.Any() ?? true) || principal.UserId == null)
            {
                return new ClaimsPrincipal();
            }
            else
            {
                var identity = new ClaimsIdentity(principal.IdentityProvider);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails ?? "NoUserDetails"));
                identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

                return new ClaimsPrincipal(identity);
            }
        }
        /*
        public static ClaimsPrincipal ParseOld(HttpRequest req)
        {
            var principal = new ClientPrincipal();

            if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
            {
                var data = header[0];
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = System.Text.Json.JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            principal.UserRoles = principal.UserRoles?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

            if ((!principal.UserRoles?.Any() ?? true) || principal.UserId == null)
            {
                return new ClaimsPrincipal();
            }

            var identity = new ClaimsIdentity(principal.IdentityProvider);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
            identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails ?? "NoUserDetails"));
            identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            return new ClaimsPrincipal(identity);
        }
        */
        public static ObjectResult Forbidden(string? errorMessage = "Error 403 Forbidden")
        {
            return new ObjectResult(errorMessage)
            {
                StatusCode = (int?)HttpStatusCode.Forbidden
            };

        }
    }
}
