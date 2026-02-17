using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportStore.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
