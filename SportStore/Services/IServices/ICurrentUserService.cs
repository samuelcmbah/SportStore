namespace SportStore.Services.IServices
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
