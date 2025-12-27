using System.Text.RegularExpressions;

namespace Infrastructure.ResetTokens;

public class EmailTemplateService(EmailTemplate template)
{
    public Task<string> Parse(Uri uri)
    {
        var result = template.Template;
        result = Regex.Replace(result, @"\${{\s*token\s*}}", uri.ToString());
        return Task.FromResult(result);
    }
}