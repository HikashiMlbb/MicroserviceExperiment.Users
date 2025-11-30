using Application.Abstractions;
using Application.Users;
using Domain.ResetTokens;
using Domain.Users;
using SharedKernel;

namespace Application.ResetTokens.Submit;

public class SubmitResetTokenHandler(IUnitOfWork uow, IPasswordService passwordService)
{
    public async Task<Result<string?>> Handle(SubmitResetToken submit)
    {
        if (string.IsNullOrEmpty(submit.Token)) return ResetTokenErrors.Empty;
        var tokenValue = new ResetTokenValue(submit.Token);

        var targetEmail = await uow.ResetTokens.Find(tokenValue);
        if (targetEmail is null) return ResetTokenErrors.NotExistsOrExpired;
        
        if (string.IsNullOrEmpty(submit.NewPassword)) return Result<string?>.Success(null);

        if (submit.NewPassword != submit.ConfirmPassword) return UserApplicationErrors.LoginFailed;

        var newPassword = UserRawPassword.Create(submit.NewPassword);
        if (!newPassword.IsSuccess) return newPassword.Error;

        var password = await passwordService.Hash(newPassword.Value);
        await uow.Users.ChangePassword(targetEmail, password);

        return string.Empty;
    }
}