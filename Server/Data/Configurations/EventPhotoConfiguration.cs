using Karakatsiya.Constants;
using Karakatsiya.Models.Entities.Audience;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class EventPhotoConfiguration : IEntityTypeConfiguration<EventPhoto>
    {
        public void Configure(EntityTypeBuilder<EventPhoto> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_URL_LENGTH);

            builder.HasOne(x => x.Event)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
