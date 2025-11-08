namespace Infrastructure.ResetTokens;

public record Message(string Topic, string Body, string Recipient);