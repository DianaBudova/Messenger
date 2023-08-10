using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Messenger.Repositories;

namespace Messenger.Validation;

public class SignUpNicknameValidationRule : ValidationRule
{
    private readonly char[] inaccessibleChars = { ' ' };
    private const byte minLength = 4;
    private const byte maxLength = 64;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string val = value.ToString()!;
        if (val.Length < SignUpNicknameValidationRule.minLength || val.Length > SignUpNicknameValidationRule.maxLength)
            return new ValidationResult(false, $"Nickname length should be from {SignUpNicknameValidationRule.minLength} to {SignUpNicknameValidationRule.maxLength} characters.");
        foreach (var ch in this.inaccessibleChars)
        {
            if (!val.Contains(ch))
                continue;
            StringBuilder stringError = new($"Nickname shouldn't contain inaccessible chars: {string.Join(", ", this.inaccessibleChars.Select(c => $"'{c}'"))}.");
            return new ValidationResult(false, stringError.ToString());
        }
        if (RepositoryFactory.GetUserRepository().ExistsNickname(val))
            return new ValidationResult(false, $"User with this nickname already exists.");
        return ValidationResult.ValidResult;
    }
}
