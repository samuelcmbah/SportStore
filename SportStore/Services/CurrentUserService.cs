using SportStore.Services.IServices;
using System.Security.Claims;

namespace SportStore.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public string? UserId =>
            IsAuthenticated
                ? _httpContextAccessor.HttpContext!.User
                    .FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

        public string Email =>
            IsAuthenticated
                ? _httpContextAccessor.HttpContext!.User
                    .FindFirstValue(ClaimTypes.Email)
                  ?? _httpContextAccessor.HttpContext!.User
                    .FindFirstValue(ClaimTypes.Name)
                  ?? "unknown@sportstore.com"
                : "guest@sportstore.com";
    }
}
