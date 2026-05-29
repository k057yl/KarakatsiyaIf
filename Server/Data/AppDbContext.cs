using Karakatsiya.Models.Entities.Admin;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.Showcase;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Karakatsiya.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventCategory> EventCategories => Set<EventCategory>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Organizer> Organizers => Set<Organizer>();
        public DbSet<AdminService> AdminServices => Set<AdminService>();
        public DbSet<EventServiceRequest> EventServiceRequests => Set<EventServiceRequest>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<EventPhoto> EventPhotos => Set<EventPhoto>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<CommentReport> CommentReports => Set<CommentReport>();
        public DbSet<TelegramConnectionCode> TelegramConnectionCodes => Set<TelegramConnectionCode>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }
        }

        private static LambdaExpression ConvertFilterExpression(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var falseConstant = Expression.Constant(false);
            var comparison = Expression.Equal(property, falseConstant);
            return Expression.Lambda(comparison, parameter);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}