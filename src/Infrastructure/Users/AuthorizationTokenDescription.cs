using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Users;

public class AuthorizationTokenDescription(IOptions<AuthorizationTokenConfig> config)
{
    public string Issuer { get; } = config.Value.Issuer;
    public string Audience { get; } = config.Value.Audience;
    public TimeSpan Expiration { get; } = config.Value.Expiration;
    public SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(config.Value.Key));
}