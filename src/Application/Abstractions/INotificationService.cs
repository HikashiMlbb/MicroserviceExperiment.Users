using Domain.ResetTokens;

namespace Application.Abstractions;

public interface INotificationService
{
    public Task Notify(ResetToken token);
}