using System.ComponentModel.DataAnnotations;

namespace SportStore.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; } = "";
    }
}
