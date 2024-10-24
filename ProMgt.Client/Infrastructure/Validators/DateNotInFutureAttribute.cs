using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Infrastructure.Validators
{
    /// <summary>
    /// Custom DataValidation Attribute
    /// </summary>
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                {
                    return new ValidationResult("Date of birth cannot be in the future.");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
