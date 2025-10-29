using Application.Abstractions;
using Application.Users;
using Domain.ResetTokens;
using Domain.Users;
using SharedKernel;

namespace Application.ResetTokens.Request;

public class RequestResetTokenHandler(IUserRepository userRepo, IResetTokenRepository tokenRepo, IResetTokenService tokenService, INotificationService notificationService)
{
    public async Task<Result> Handle(RequestResetToken request)
    {
        var emailResult = UserEmail.Create(request.Email);
        if (!emailResult.IsSuccess) return ResetTokenErrors.ValidationError;

        var isExists = await userRepo.IsExists(emailResult.Value);
        if (!isExists) return UserApplicationErrors.NotFound;

        var isRequested = await tokenRepo.IsRequested(emailResult.Value);
        if (isRequested) return ResetTokenErrors.AlreadyRequested;

        var tokenValue = await tokenService.Generate();
        var tokenExpiration = await tokenService.GetExpiration();
        var token = new ResetToken(emailResult.Value, tokenValue, tokenExpiration);
        await tokenRepo.Save(token);
        await notificationService.Notify(token);
        
        return Result.Success();
    }
}