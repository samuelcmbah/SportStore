using System.ComponentModel.DataAnnotations;

namespace SportStore.ViewModels.Role
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; } = "";
    }
}
