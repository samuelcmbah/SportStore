using CloudinaryDotNet.Actions;

namespace SportStore.Services.IServices
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
        string? ExtractPublicIdFromUrl(string? imageUrl);
    }
}
