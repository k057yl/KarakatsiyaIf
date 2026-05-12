using Karakatsiya.Constants;
using Karakatsiya.Services.Interfaces;

namespace Karakatsiya.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AppConstants.Storage.ALLOWED_EXTENSIONS.Contains(extension))
                throw new Exception("ERRORS.INVALID_FILE_TYPE");

            var fileName = $"{Guid.NewGuid()}{extension}";

            var uploadsPath = Path.Combine(_env.WebRootPath, AppConstants.Storage.UPLOADS_FOLDER, subFolder);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fullPath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{AppConstants.Storage.UPLOADS_FOLDER}/{subFolder}/{fileName}";
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            var cleanPath = relativePath.TrimStart('/');
            var fullPath = Path.Combine(_env.ContentRootPath, AppConstants.Storage.WWWROOT_FOLDER, cleanPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
