namespace Application.Abstractions;

public interface IAuthorizationTokenService
{
    public Task<string> GenerateToken(AuthorizationTokenPayload payload);
}