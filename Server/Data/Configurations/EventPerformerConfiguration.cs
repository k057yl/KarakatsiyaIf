using Karakatsiya.Data.Entities.Showcase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class EventPerformerConfiguration : IEntityTypeConfiguration<EventPerformer>
    {
        public void Configure(EntityTypeBuilder<EventPerformer> builder)
        {
            builder.ToTable("EventPerformers");

            builder.HasKey(ep => new { ep.EventId, ep.PerformerId });

            builder.HasOne(ep => ep.Event)
                .WithMany(e => e.EventPerformers)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.Performer)
                .WithMany(p => p.EventPerformers)
                .HasForeignKey(ep => ep.PerformerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
