

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PhoneNumberValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Telefon numarası zorunludur.");
        }

        // Telefon numarasını string olarak al
        string phoneNumber = value.ToString();

        // Parantez, boşluk ve özel karakterleri kaldır
        phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", ""); // Sadece rakamları bırak

        System.Console.WriteLine(phoneNumber);

        // Telefon numarasının uzunluğunu kontrol et
        if (phoneNumber.Length != 10)
        {
            return new ValidationResult("Telefon numarası 10 karakter uzunluğunda olmalıdır.");
        }

        // Telefon numarasının yalnızca rakamlardan oluşup oluşmadığını kontrol et
        if (!long.TryParse(phoneNumber, out _))
        {
            return new ValidationResult("Telefon numarası yalnızca rakamlardan oluşmalıdır.");
        }

        return ValidationResult.Success;
    }
}


