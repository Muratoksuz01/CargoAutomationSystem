using System.ComponentModel.DataAnnotations;

namespace CargoAutomationSystem.Models.Corporate
{
    public class BSettingsViewModel
    {
        public BUpdateUsernameViewModel? UpdateUsername { get; set; }
        public BUpdatePasswordViewModel? UpdatePassword { get; set; }
        public BEditInfoViewModel ? EditInfo {get;set;}


    }

    public class BEditInfoViewModel(){
         [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
      
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
    }

    public class BUpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Re-entering the new password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }

    public class BUpdateUsernameViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; }
    }


    


    


    

}