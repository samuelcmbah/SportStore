using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SportStore.Configurations;
using SportStore.Services.IServices;

namespace SportStore.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IOptions<CloudinarySettings> config;
        private readonly ILogger<CloudinaryService> logger;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(
            IOptions<CloudinarySettings> config,
            ILogger<CloudinaryService> logger)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
            this.logger = logger;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if(result.Error != null)
            {
                logger.LogError("Cloudinary deletion error: {ErrorMessage}", result.Error.Message);
            }
            return result;
        }

        public string? ExtractPublicIdFromUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return null;
            }

            // Cloudinary URLs look like:
            // https://res.cloudinary.com/cloud-name/image/upload/v1234567/sportstore/products/filename.jpg
            // We need: sportstore/products/filename

            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/');

                //Find the upload segment
                int uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1)
                    return null;

                //skip version (v1234567) if present 
                int startIndex = uploadIndex + 1;
                if (segments.Length > startIndex && segments[startIndex].StartsWith("v"))
                {
                    startIndex++;
                }

                //join remaining segments (folder/filename)
                var publicIdWithExtension = string.Join('/', segments.Skip(startIndex));

                //remove file extension
                var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    return publicIdWithExtension.Substring(0, lastDotIndex);
                }
                return publicIdWithExtension;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error extracting public ID from Cloudinary URL: {ImageUrl}", imageUrl);
                return null;
            }
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult(); 

            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                        .Width(800)
                        .Height(800)
                        .Crop("limit")
                        .Quality("auto"),
                    Folder = "sportstore/products",

                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                }
            }
            return uploadResult;
        }
    }
}
