using System;
using System.Collections.Generic;
namespace BlazorApp.Shared
{
    public class ClientPrincipal
    {
        public string? IdentityProvider { get; init; }
        public string? UserId { get; init; }
        public string? UserDetails { get; init; }
        public IEnumerable<string>? UserRoles { get; set; }
    }
}
