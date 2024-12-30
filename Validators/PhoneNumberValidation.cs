using System.ComponentModel.DataAnnotations;



    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Telefon numarası zorunludur.");
            }

            string phoneNumber = value.ToString();

            if (phoneNumber.Length != 10)
            {
                return new ValidationResult("Telefon numarası 10 karakter uzunluğunda olmalıdır.");
            }

            if (!long.TryParse(phoneNumber, out _))
            {
                return new ValidationResult("Telefon numarası yalnızca rakamlardan oluşmalıdır.");
            }

            return ValidationResult.Success;
        }
    }

