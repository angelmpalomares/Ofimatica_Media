
using System.Text.RegularExpressions;

namespace Infrastructure.Validation
{
    public static class UserValidator
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PasswordRegex = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
            RegexOptions.Compiled);
        public static ValidationResult ValidateEmail(string? email, bool isUpdate)
        {
            var errors = new List<string>();
            if (isUpdate && string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success();
            }
            if (!isUpdate && string.IsNullOrWhiteSpace(email))
            {
                errors.Add("ErrorMessage.EmailIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (!EmailRegex.IsMatch(email)) 
            {
                errors.Add("ErrorMessage.WrongEmailFormat");
                return ValidationResult.Failure(errors);
            }
            return ValidationResult.Success();
        }

        public static ValidationResult ValidatePassword(string? password, bool isUpdate)
        {
            var errors = new List<string>();
            if (isUpdate && string.IsNullOrWhiteSpace(password))
            {
                return ValidationResult.Success();
            }

            if (!isUpdate && string.IsNullOrWhiteSpace(password))
            {
                errors.Add("ErrorMessage.PasswordIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (!PasswordRegex.IsMatch(password))
            {
                errors.Add("ErrorMessage.PasswordDoesntMeetRequirements");
            }

            return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
        }
    }
}
