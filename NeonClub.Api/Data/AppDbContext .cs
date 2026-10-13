using Microsoft.EntityFrameworkCore;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<Zone> Zones => Set<Zone>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Image> Images => Set<Image>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            mb.Entity<Role>(e =>
            {
                e.ToTable("roles");
                e.HasKey(r => r.Id);
                e.Property(r => r.Name).IsRequired().HasMaxLength(50);
                e.HasIndex(r => r.Name).IsUnique();
            });

            mb.Entity<Status>(e =>
            {
                e.ToTable("statuses");
                e.HasKey(s => s.Id);
                e.Property(s => s.Name).IsRequired().HasMaxLength(50);
                e.Property(s => s.Description).HasMaxLength(100);
                e.HasIndex(s => s.Name).IsUnique();
            });

            mb.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(u => u.Id);
                e.Property(u => u.Id).ValueGeneratedNever();

                e.Property(u => u.Name).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(255);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

                e.HasOne(u => u.Role)
                 .WithMany()
                 .HasForeignKey(u => u.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<Booking>(e =>
            {
                e.ToTable("bookings");
                e.HasKey(b => b.Id);
                e.Property(b => b.Id).ValueGeneratedNever();

                e.Property(b => b.Hours).IsRequired();
                e.Property(b => b.Total).HasColumnType("numeric(10,2)");

                e.HasOne(b => b.User)
                 .WithMany(u => u.Bookings)
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(b => b.Zone)
                 .WithMany(z => z.Bookings)
                 .HasForeignKey(b => b.ZoneId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Status)
                 .WithMany()
                 .HasForeignKey(b => b.StatusId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(b => b.UserId);
                e.HasIndex(b => b.ZoneId);
                e.HasIndex(b => b.CreatedAt);
            });

            mb.Entity<Zone>(e =>
            {
                e.ToTable("zones");

                e.HasKey(z => z.Id);
                e.Property(z => z.Id)
                    .ValueGeneratedNever();

                e.Property(z => z.Title)
                    .IsRequired()
                    .HasMaxLength(50);
                e.HasIndex(z => z.Title)
                    .IsUnique();

                e.Property(z => z.Subtitle)
                    .HasMaxLength(255);

                e.Property(z => z.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(z => z.Price)
                    .IsRequired()
                    .HasPrecision(10, 2);

                e.Property(z => z.Emoji)
                    .HasMaxLength(10);

                e.Property(z => z.Tag)
                    .HasMaxLength(100);

                e.Property(z => z.Cpu)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(z => z.Gpu)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(z => z.Ram)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(z => z.Monitor)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(z => z.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                e.Property(z => z.Features)
                    .HasColumnType("text[]");

                e.HasIndex(z => z.Category);
            });

            mb.Entity<Image>(e =>
            {
                e.ToTable("images");
                e.HasKey(i => i.Id);
                e.Property(i => i.Id).ValueGeneratedNever();

                e.Property(i => i.OriginalName).IsRequired().HasMaxLength(255);
                e.Property(i => i.Path).IsRequired().HasMaxLength(500);
                e.Property(i => i.MimeType).HasMaxLength(100);

                e.HasOne(i => i.Zone)
                    .WithMany()
                    .HasForeignKey(i => i.ZoneId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(i => i.ZoneId);
            });
        }
    }
}
