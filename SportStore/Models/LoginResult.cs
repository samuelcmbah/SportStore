using Microsoft.AspNetCore.Identity;

namespace SportStore.Models
{
    public class LoginResult
    {
        public required SignInResult SignInResult { get; set; }
        public bool IsAdmin { get; set; }
    }
}
