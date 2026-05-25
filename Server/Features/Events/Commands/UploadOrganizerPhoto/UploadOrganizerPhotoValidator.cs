using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public class UploadOrganizerPhotoValidator : AbstractValidator<UploadOrganizerPhotoCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024;

        public UploadOrganizerPhotoValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);
            RuleFor(x => x.UserId).NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.File)
                .NotNull().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file => file.Length > 0).WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file => file.Length <= MaxFileSize).WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file =>
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return AllowedExtensions.Contains(ext);
                }).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
