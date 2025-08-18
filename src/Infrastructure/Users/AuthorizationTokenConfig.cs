namespace Infrastructure.Users;

public class AuthorizationTokenConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public TimeSpan Expiration { get; set; }
    public string Key { get; set; } = null!;
}