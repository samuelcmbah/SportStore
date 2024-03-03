using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string ReturnUrl { get; set; } = "/";
    }
}
