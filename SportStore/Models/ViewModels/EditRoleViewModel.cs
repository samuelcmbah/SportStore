using System.ComponentModel.DataAnnotations;

namespace SportStore.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public string? Id { get; set; }

        [Required]
        public string? RoleName { get; set; }

        public List<string> Users { get; set; } = new();
    }
}
