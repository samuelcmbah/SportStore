using Microsoft.AspNetCore.Identity;

namespace SportStore.Models.DTOs
{
    public class LoginResultDTO
    {
        public required SignInResult SignInResult { get; set; }
        public bool IsAdmin { get; set; }
    }
}
