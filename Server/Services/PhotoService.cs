using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Karakatsiya.Constants;
using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Services.Interfaces;
using Microsoft.Extensions.Options;
using SkiaSharp;
using CloudinaryError = CloudinaryDotNet.Actions.Error;

namespace Karakatsiya.Services.Infrastructure
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<PhotoService> _logger;

        private const int MainTargetWidth = 1200;
        private const int GalleryTargetWidth = 800;
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

        public PhotoService(IOptions<CloudinarySettings> config, ILogger<PhotoService> logger)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
            _logger = logger;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, bool isMain)
        {
            if (file == null || file.Length == 0)
                return new ImageUploadResult();

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                _logger.LogWarning("Попытка загрузить неверный формат файла: {Type}", file.ContentType);
                return new ImageUploadResult { Error = new CloudinaryError { Message = AppConstants.Errors.VALIDATION_FAILED } };
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return new ImageUploadResult { Error = new CloudinaryError { Message = AppConstants.Errors.VALIDATION_FAILED } };
            }

            using var outStream = new MemoryStream();
            try
            {
                using (var inputStream = file.OpenReadStream())
                using (var originalBitmap = SKBitmap.Decode(inputStream))
                {
                    if (originalBitmap == null)
                    {
                        return new ImageUploadResult { Error = new CloudinaryError { Message = AppConstants.Errors.VALIDATION_FAILED } };
                    }

                    int targetWidth = isMain ? MainTargetWidth : GalleryTargetWidth;
                    int targetHeight = (int)(originalBitmap.Height * ((float)targetWidth / originalBitmap.Width));

                    if (originalBitmap.Width <= targetWidth)
                    {
                        targetWidth = originalBitmap.Width;
                        targetHeight = originalBitmap.Height;
                    }

                    var imageInfo = new SKImageInfo(targetWidth, targetHeight);
                    using (var resizedBitmap = new SKBitmap(imageInfo))
                    {
                        originalBitmap.ScalePixels(resizedBitmap, new SKSamplingOptions(SKCubicResampler.Mitchell));

                        using (var image = SKImage.FromBitmap(resizedBitmap))
                        using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 82))
                        {
                            data.SaveTo(outStream);
                        }
                    }
                }

                outStream.Position = 0;
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, outStream),
                    Folder = "Karakatsiya_Events",
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };

                return await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Photo processing via SkiaSharp failed");
                return new ImageUploadResult { Error = new CloudinaryError { Message = AppConstants.Errors.VALIDATION_FAILED } };
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            return await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        }
    }
}