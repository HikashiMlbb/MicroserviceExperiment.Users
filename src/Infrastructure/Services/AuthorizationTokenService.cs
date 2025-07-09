using Application.Abstractions;

namespace Infrastructure.Services;

public class AuthorizationTokenService : IAuthorizationTokenService
{
    public Task<string> GenerateToken(AuthorizationTokenPayload payload)
    {
        throw new NotImplementedException();
    }
}