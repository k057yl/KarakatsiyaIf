using Karakatsiya.Models.Entities.Admin;
using Karakatsiya.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class EventServiceRequestConfiguration : IEntityTypeConfiguration<EventServiceRequest>
    {
        public void Configure(EntityTypeBuilder<EventServiceRequest> builder)
        {
            builder.HasKey(x => new { x.EventId, x.AdminServiceId });

            builder.Property(x => x.AgreedPrice)
                .HasPrecision(AppConstants.Validation.DECIMAL_PRECISION,
                              AppConstants.Validation.DECIMAL_SCALE);

            builder.HasOne(x => x.Event)
                .WithMany(x => x.ServiceRequests)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AdminService)
                .WithMany()
                .HasForeignKey(x => x.AdminServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}