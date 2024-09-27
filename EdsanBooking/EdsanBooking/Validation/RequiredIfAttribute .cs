using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _conditionPropertyName;
        private readonly string _conditionValue;

        public RequiredIfAttribute(string conditionPropertyName, string conditionValue)
        {
            _conditionPropertyName = conditionPropertyName;
            _conditionValue = conditionValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conditionProperty = validationContext.ObjectType.GetProperty(_conditionPropertyName);
            if (conditionProperty == null)
            {
                return new ValidationResult($"Unknown property: {_conditionPropertyName}");
            }

            var conditionValue = conditionProperty.GetValue(validationContext.ObjectInstance)?.ToString();
            if (conditionValue == _conditionValue && value == null)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
            }

            return ValidationResult.Success;
        }
    }
}
