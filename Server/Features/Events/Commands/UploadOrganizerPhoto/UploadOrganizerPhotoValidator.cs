using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public class UploadOrganizerPhotoValidator : AbstractValidator<UploadOrganizerPhotoCommand>
    {
        public UploadOrganizerPhotoValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.File)
                .NotNull().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file => file.Length > 0).WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file => file.Length <= AppConstants.Storage.MAX_FILE_SIZE_BYTES)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .Must(file =>
                {
                    if (file == null || string.IsNullOrEmpty(file.FileName)) return false;
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return AppConstants.Storage.ALLOWED_EXTENSIONS.Contains(ext);
                }).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
