namespace Infrastructure.Validation
{
    public sealed class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }
        private ValidationResult(bool isValid, List<string> errors) 
        {
            IsValid = isValid;
            Errors = errors;
        }
        public static ValidationResult Success()
        {
            return new ValidationResult(true, []);
        }
        public static ValidationResult Failure(List<string> errors) 
        { 
            return new ValidationResult(false, errors); 
        }
    }
}
