using System.Globalization;
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
        if (val.Length < minLength || val.Length > maxLength)
            return new ValidationResult(false, $"Nickname length should be from {minLength} to {maxLength} characters");
        foreach (var ch in this.inaccessibleChars)
        {
            if (val.Contains(ch))
            {
                StringBuilder stringError = new($"Nickname shouldn't contain inaccessible chars: ");
                foreach (var ch2 in this.inaccessibleChars)
                    stringError.Append($"'{ch2}', ");
                return new ValidationResult(false, stringError.Remove(stringError.Length - 2, 2).ToString());
            }
        }
        return ValidationResult.ValidResult;
    }
}