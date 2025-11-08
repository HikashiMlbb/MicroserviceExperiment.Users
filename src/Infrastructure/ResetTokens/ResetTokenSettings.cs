namespace Infrastructure.ResetTokens;

public class ResetTokenSettings
{
    public TimeSpan Expiration { get; init; }
    public string Url { get; init; } = null!;
}