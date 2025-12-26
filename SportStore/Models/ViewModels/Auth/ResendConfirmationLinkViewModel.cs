using System.ComponentModel.DataAnnotations;

namespace SportStore.Models.ViewModels.Auth
{
    public class ResendConfirmationLinkViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }
}
