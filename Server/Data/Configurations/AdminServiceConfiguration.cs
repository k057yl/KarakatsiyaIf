using Karakatsiya.Models.Entities.Admin;
using Karakatsiya.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class AdminServiceConfiguration : IEntityTypeConfiguration<AdminService>
    {
        public void Configure(EntityTypeBuilder<AdminService> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_NAME_LENGTH);

            builder.Property(x => x.BasePrice)
                .HasPrecision(AppConstants.Validation.DECIMAL_PRECISION,
                              AppConstants.Validation.DECIMAL_SCALE);
        }
    }
}