using System;
using System.Collections.Generic;
using Application.Common.Util;
using Domain.Entities;
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
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var managerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();

                entity.HasData(
                    new Role { Id = adminRoleId, Name = "Admin" },
                    new Role { Id = managerRoleId, Name = "Manager" }
                );
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Users");

                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.Password).HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasData(new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "DCSE",
                    Password = PasswordUtil.HashPassword("Admin123@"),
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    IsDeleted = false,
                    IsLock = false,
                    RoleId = adminRoleId,
                    IsUseSoftWare = true
                });
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Permissions");

                entity.Property(e => e.Action).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Module).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}