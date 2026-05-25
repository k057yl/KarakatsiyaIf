

using CloudinaryDotNet.Actions;

namespace Karakatsiya.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file, bool isMain);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
