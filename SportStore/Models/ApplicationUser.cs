using Microsoft.AspNetCore.Identity;

namespace SportStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
