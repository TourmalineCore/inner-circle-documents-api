using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using DataAccess;

namespace Api.Configuration;

public static class AuthenticationExtensions
{
    public static void AddAppAuthentication(this WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
        builder.Services.AddJwtAuthentication(authenticationOptions)
            .WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);
        builder.Services.AddPersistence(configuration);
    }
}