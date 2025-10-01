using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Infrastructure.Validation
{
    public static class ResourceValidator
    {
        // Description: solo letras, números, espacios, comas, guiones, punto y coma y puntos.
        // Además, bloquea cualquier intento de meter etiquetas HTML.
        private static readonly Regex DescriptionRegex = new(
            @"^(?!.*<[^>]+>)[a-zA-Z0-9\s,;\-\.]+$",
            RegexOptions.Compiled);

        // Author: solo letras, espacios, comas, guiones, punto y coma y puntos.
        // Además, bloquea cualquier intento de meter etiquetas HTML.
        private static readonly Regex AuthorRegex = new(
            @"^(?!.*<[^>]+>)[a-zA-Z\s,;\-\.]+$",
            RegexOptions.Compiled);

        // Name: puede ser más flexible (permitir caracteres especiales de títulos),
        // pero bloqueamos cualquier etiqueta HTML (<script>, <img>, <iframe>, etc.)
        private static readonly Regex NameRegex = new(
            @"^[^<>]+$",
            RegexOptions.Compiled);

        public static ValidationResult ValidateAuthor(string? author, bool isUpdate)
        {
            var errors = new List<string>();

            if (isUpdate && string.IsNullOrWhiteSpace(author))
            {
                return ValidationResult.Success();
            }
            if (!isUpdate && string.IsNullOrWhiteSpace(author))
            {
                errors.Add("ErrorMessage.AuthorIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (!AuthorRegex.IsMatch(author))
            {
                errors.Add("ErrorMessage.AuthorInvalidCharacters");
            }

            return errors.Any()
                ? ValidationResult.Failure(errors)
                : ValidationResult.Success();
        }

        public static ValidationResult ValidateDescription(string? description, bool isUpdate)
        {
            var errors = new List<string>();

            if (isUpdate && string.IsNullOrWhiteSpace(description))
            {
                return ValidationResult.Success();
            }
            if (!isUpdate && string.IsNullOrWhiteSpace(description))
            {
                errors.Add("ErrorMessage.DescriptionIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (!DescriptionRegex.IsMatch(description))
            {
                errors.Add("ErrorMessage.DescriptionInvalidCharacters");
            }

            return errors.Any()
                ? ValidationResult.Failure(errors)
                : ValidationResult.Success();
        }

        public static ValidationResult ValidateName(string? name, bool isUpdate)
        {
            var errors = new List<string>();

            if (isUpdate && string.IsNullOrWhiteSpace(name))
            {
                return ValidationResult.Success();
            }
            if (!isUpdate && string.IsNullOrWhiteSpace(name))
            {
                errors.Add("ErrorMessage.NameIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (!NameRegex.IsMatch(name))
            {
                errors.Add("ErrorMessage.NameContainsScript");
            }

            return errors.Any()
                ? ValidationResult.Failure(errors)
                : ValidationResult.Success();
        }

        public static ValidationResult ValidateYear(int? year, bool isUpdate)
        {
            var errors = new List<string>();

            if (isUpdate && !year.HasValue)
            {
                return ValidationResult.Success();
            }
            if (!isUpdate && !year.HasValue)
            {
                errors.Add("ErrorMessage.YearIsEmpty");
                return ValidationResult.Failure(errors);
            }
            if (year > DateTime.UtcNow.Year)
            {
                errors.Add("ErrorMessage.InvalidYear");
            }

            return errors.Any()
                ? ValidationResult.Failure(errors)
                : ValidationResult.Success();
        }
    }
}
