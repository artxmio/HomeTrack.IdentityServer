using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using static System.Net.WebRequestMethods;

namespace HomeTrack.IdentityServer;

public class Configuration
{
    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new("HomeTrackWebAPI", "Web API")
        ];

    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
            new("HomeTrackWebApi", "Web API",
                [JwtClaimTypes.Name])
            {
                Scopes = { "HomeTrackWebAPI" }
            }
        ];

    public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientId = "hometrack-web-api",
                ClientName = "HomeTrack Web",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris =
                {
                    "http://.../signin-oidc"
                },
                AllowedCorsOrigins =
                {
                    "http://..."
                },
                PostLogoutRedirectUris =
                {
                   "http://.../signout-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "HomeTrackWebAPI"
                },
                AllowAccessTokensViaBrowser = true,
            }
        ];
}
