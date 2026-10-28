using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Util;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Role)
                      .HasConversion<string>();

                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.HasIndex(e => e.UserName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0 AND [UserName] IS NOT NULL")
                      .IncludeProperties(e => new { e.Password, e.Role, e.IsLock });

                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0")
                      .IncludeProperties(e => new { e.UserName });

                entity.HasIndex(e => e.Code)
                      .HasFilter("[IsDeleted] = 0")
                      .IncludeProperties(e => new { e.UserName });

                entity.HasIndex(e => new { e.IsDeleted, e.CreatedDate })
                      .IsDescending(false, true)
                      .IncludeProperties(e => new { e.Code, e.UserName, e.Email, e.Phone, e.Role, e.IsLock, e.Avatar });
            });

            // Global soft delete query filter for all BaseEntity
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);

            // Seed initial SYSTEM and USER accounts
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("467389cb-7b19-4aa8-b11c-2f928e078ba1"),
                    Avatar = "https://marketplace.canva.com/tXH-Q/MAG7IGtXH-Q/1/tl/canva-MAG7IGtXH-Q.jpg",
                    Code = "DCSE-DEV",
                    IsDeleted = false,
                    IsLock = false,
                    Email = "dcse@dcs.vn",
                    Phone = "0372599558",
                    Password = PasswordUtil.HashPassword("Admin123@"),
                    Role = ERole.SYSTEM,
                    UserName = "DCSE",
                    CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "DCSE",
                    LastModifiedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    LastModifiedBy = "DCSE"
                },
                new User
                {
                    Id = new Guid("62a9f050-9e65-4cbe-89d2-29accff5d0cb"),
                    Avatar = "https://marketplace.canva.com/tXH-Q/MAG7IGtXH-Q/1/tl/canva-MAG7IGtXH-Q.jpg",
                    Code = "DCS-USER",
                    IsDeleted = false,
                    IsLock = false,
                    Email = "user@dcs.vn",
                    Phone = "0386040060",
                    Password = PasswordUtil.HashPassword("Admin123@"),
                    Role = ERole.USER,
                    UserName = "USER",
                    CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "SYSTEM",
                    LastModifiedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    LastModifiedBy = "DCSE"
                }
            );

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity<Guid>>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
