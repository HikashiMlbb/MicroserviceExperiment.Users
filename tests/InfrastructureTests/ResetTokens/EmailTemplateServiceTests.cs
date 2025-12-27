using Infrastructure.ResetTokens;

namespace InfrastructureTests.ResetTokens;

[TestFixture]
public class EmailTemplateServiceTests
{
    [Test]
    public async Task Replace_Url_And_Token()
    {
        // Arrange
        const string html = "<h1>We're detected you're trying to reset password. If it was you, please, follow the link below:</h1><p><a href=\"${{ token }}\"></p><p>But if it wasn't you, please, ignore this email.</p><p>Best wishes, Hikashi no Development!</p>";
        const string url = "http://some.domain.com/tokens/123456-abcdef-987654-fedcba";
        var template = new EmailTemplate { Template = html };
        var svc = new EmailTemplateService(template);
        
        // Act
        var result = await svc.Parse(new Uri(url));

        // Assert
        Assert.That(result, Is.EqualTo(html.Replace("${{ token }}", url)));
    }
}