using CloudinaryDotNet.Actions;

namespace SportStore.Services.IServices
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeletionAsync(IFormFile file);
        string? ExtractPublicIdFromUrl(string? imageUrl);
    }
}
