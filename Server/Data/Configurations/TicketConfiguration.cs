using Karakatsiya.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Karakatsiya.Data.Entities.Audience;

namespace Karakatsiya.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketCode)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_TICKET_CODE_LENGTH);

            builder.HasIndex(x => x.TicketCode).IsUnique();

            builder.HasOne(x => x.Event)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}