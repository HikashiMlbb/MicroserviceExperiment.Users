using SharedKernel;

namespace Domain.ResetTokens;

public sealed record ResetTokenExpiration
{
    public DateTime Value { get; init; }

    private static readonly TimeSpan MinExpiration = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MaxExpiration = TimeSpan.FromDays(1);
    
    private ResetTokenExpiration(DateTime value)
    {
        Value = value;
    }

    public static Result<ResetTokenExpiration> Create(DateTime value)
    {
        var timespan = value - DateTime.UtcNow;
        if (timespan < MinExpiration || timespan > MaxExpiration)
        {
            return ResetTokenDomainErrors.ExpirationOutOfRange;
        }

        return new ResetTokenExpiration(value);
    }
}