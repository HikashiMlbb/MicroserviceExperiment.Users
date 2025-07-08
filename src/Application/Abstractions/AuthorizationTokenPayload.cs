using Domain.Users;

namespace Application.Abstractions;

public record AuthorizationTokenPayload(UserId Id);