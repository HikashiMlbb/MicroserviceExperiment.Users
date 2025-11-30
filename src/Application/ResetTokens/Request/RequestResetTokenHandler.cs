using Application.Abstractions;
using Application.Users;
using Domain.ResetTokens;
using Domain.Users;
using SharedKernel;

namespace Application.ResetTokens.Request;

public class RequestResetTokenHandler(IUnitOfWork uow, IResetTokenService tokenService, INotificationService notificationService)
{
    public async Task<Result> Handle(RequestResetToken request)
    {
        var emailResult = UserEmail.Create(request.Email);
        if (!emailResult.IsSuccess) return ResetTokenErrors.ValidationError;

        var isExists = await uow.Users.IsExists(emailResult.Value);
        if (!isExists) return UserApplicationErrors.NotFound;

        var isRequested = await uow.ResetTokens.IsRequested(emailResult.Value);
        if (isRequested) return ResetTokenErrors.AlreadyRequested;

        var tokenValue = await tokenService.Generate();
        var tokenExpiration = await tokenService.GetExpiration();
        var token = new ResetToken(emailResult.Value, tokenValue, tokenExpiration);
        await uow.ResetTokens.Save(token);
        await notificationService.Notify(token);
        
        return Result.Success();
    }
}