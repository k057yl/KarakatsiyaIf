using Karakatsiya.Constants;
using Karakatsiya.Data.Entities.Showcase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_NAME_LENGTH);

            builder.HasIndex(x => x.OsmId).IsUnique();

            builder.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.City)
                    .HasMaxLength(AppConstants.Validation.MAX_CITY_LENGTH)
                    .HasColumnName(AppConstants.Columns.ADDRESS_CITY);

                a.Property(p => p.Street)
                    .HasMaxLength(AppConstants.Validation.MAX_STREET_LENGTH)
                    .HasColumnName(AppConstants.Columns.ADDRESS_STREET);

                a.Property(p => p.HouseNumber)
                    .HasMaxLength(AppConstants.Validation.MAX_HOUSE_NUMBER_LENGTH)
                    .HasColumnName(AppConstants.Columns.ADDRESS_HOUSE);
            });
        }
    }
}
