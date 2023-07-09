using System.Globalization;
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
        string val = value.ToString()!;
        if (val.Length < minLength || val.Length > maxLength)
            return new ValidationResult(false, $"Password length should be from {minLength} to {maxLength} characters");
        foreach (var ch in this.inaccessibleChars)
        {
            if (val.Contains(ch))
            {
                StringBuilder stringError = new($"Password shouldn't contain inaccessible chars: ");
                foreach (var ch2 in this.inaccessibleChars)
                    stringError.Append($"\"{ch2}\", ");
                return new ValidationResult(false, stringError.Remove(stringError.Length - 2, 2).ToString());
            }
        }
        return ValidationResult.ValidResult;
    }
}
