using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Messenger.Validation;

public class SignInNicknameValidationRule : ValidationRule
{
    private readonly char[] inaccessibleChars = { ' ' };
    private const byte minLength = 4;
    private const byte maxLength = 64;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string val = value.ToString()!;
        if (val.Length < SignInNicknameValidationRule.minLength || val.Length > SignInNicknameValidationRule.maxLength)
            return new ValidationResult(false, $"Nickname length should be from {SignInNicknameValidationRule.minLength} to {SignInNicknameValidationRule.maxLength} characters.");
        foreach (var ch in this.inaccessibleChars)
        {
            if (!val.Contains(ch))
                continue;
            StringBuilder stringError = new($"Nickname shouldn't contain inaccessible chars: {string.Join(", ", this.inaccessibleChars.Select(c => $"'{c}'"))}.");
            return new ValidationResult(false, stringError.ToString());
        }
        return ValidationResult.ValidResult;
    }
}
