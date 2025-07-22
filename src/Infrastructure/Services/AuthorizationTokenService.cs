using System.Security.Claims;
using Application.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthorizationTokenService(AuthorizationTokenDescription tokenDescription) : IAuthorizationTokenService
{
    public Task<string> GenerateToken(AuthorizationTokenPayload payload)
    {
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, payload.Id.Value.ToString()) };
        var token = new SecurityTokenDescriptor
        {
            Issuer = tokenDescription.Issuer,
            Audience = tokenDescription.Audience,
            Expires = DateTime.UtcNow + tokenDescription.Expiration,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(tokenDescription.Key, SecurityAlgorithms.HmacSha384)
        };

        return Task.FromResult(new JsonWebTokenHandler().CreateToken(token));
    }
}