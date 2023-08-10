using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Messenger.Validation;

public class PasswordValidationRule : ValidationRule
{
    private readonly char[] inaccessibleChars = { ' ' };
    private const byte minLength = 8;
    private const byte maxLength = 16;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string? val = value?.ToString();
        if (val is null)
            return new ValidationResult(false, "Something went wrong.");
        if (val.Length < PasswordValidationRule.minLength || val.Length > PasswordValidationRule.maxLength)
            return new ValidationResult(false, $"Password length should be from {PasswordValidationRule.minLength} to {PasswordValidationRule.maxLength} characters.");
        foreach (var ch in this.inaccessibleChars)
        {
            if (!val.Contains(ch))
                continue;
            StringBuilder stringError = new($"Password shouldn't contain inaccessible chars: {string.Join(", ", this.inaccessibleChars.Select(c => $"'{c}'"))}.");
            return new ValidationResult(false, stringError.ToString());
        }
        return ValidationResult.ValidResult;
    }
}
