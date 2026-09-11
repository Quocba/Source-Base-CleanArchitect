using System;
using System.Collections.Generic;
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

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Category Configuration
            var electronicsCatId = Guid.Parse("11111111-aaaa-bbbb-cccc-111111111111");
            var householdCatId = Guid.Parse("22222222-aaaa-bbbb-cccc-222222222222");
            var fashionCatId = Guid.Parse("33333333-aaaa-bbbb-cccc-333333333333");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Categories");

                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasData(
                    new Category
                    {
                        Id = electronicsCatId,
                        Code = "CAT_DIENTU",
                        Name = "Thiết bị điện tử",
                        Description = "Điện thoại, laptop, phụ kiện công nghệ",
                        IsActive = true,
                        CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = householdCatId,
                        Code = "CAT_GIADUNG",
                        Name = "Đồ gia dụng thông minh",
                        Description = "Thiết bị nhà bếp và đời sống gia đình",
                        IsActive = true,
                        CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = fashionCatId,
                        Code = "CAT_THOITRANG",
                        Name = "Thời trang & Phụ kiện",
                        Description = "Quần áo, giày dép cao cấp",
                        IsActive = true,
                        CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    }
                );
            });
            #endregion

            #region Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Products");

                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.IsDeleted, e.Status, e.CreatedDate });

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasData(
                    new Product
                    {
                        Id = Guid.Parse("aaaaaaaa-1111-2222-3333-aaaaaaaaaaaa"),
                        Code = "PROD-001",
                        Name = "Bàn phím cơ không dây Bluetooth 5.0",
                        Description = "Bàn phím cơ Custom 75%, hot-swap, RGB",
                        Price = 1450000,
                        StockQuantity = 120,
                        Status = ProductStatus.Active,
                        CategoryId = electronicsCatId,
                        CreatedDate = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.Parse("bbbbbbbb-1111-2222-3333-bbbbbbbbbbbb"),
                        Code = "PROD-002",
                        Name = "Chuột gaming công thái học không dây",
                        Description = "Cảm biến quang học 26,000 DPI, siêu nhẹ 55g",
                        Price = 1890000,
                        StockQuantity = 85,
                        Status = ProductStatus.Active,
                        CategoryId = electronicsCatId,
                        CreatedDate = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.Parse("cccccccc-1111-2222-3333-cccccccccccc"),
                        Code = "PROD-003",
                        Name = "Nồi chiên không dầu điện tử 6.5L",
                        Description = "Công nghệ đốt nóng đối lưu 360 độ, lòng nồi chống dính",
                        Price = 2200000,
                        StockQuantity = 40,
                        Status = ProductStatus.Active,
                        CategoryId = householdCatId,
                        CreatedDate = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.Parse("dddddddd-1111-2222-3333-dddddddddddd"),
                        Code = "PROD-004",
                        Name = "Máy hút bụi cầm tay lực hút 20,000Pa",
                        Description = "Pin sạc lithium-ion 4000mAh, đa đầu hút tiện lợi",
                        Price = 1650000,
                        StockQuantity = 60,
                        Status = ProductStatus.Active,
                        CategoryId = householdCatId,
                        CreatedDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.Parse("eeeeeeee-1111-2222-3333-eeeeeeeeeeee"),
                        Code = "PROD-005",
                        Name = "Áo khoác gió thể thao chống nước",
                        Description = "Chất liệu Gore-Tex thoáng khí, chống gió bụi tối ưu",
                        Price = 750000,
                        StockQuantity = 250,
                        Status = ProductStatus.Active,
                        CategoryId = fashionCatId,
                        CreatedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    }
                );
            });
            #endregion

            #region Role & User Configuration
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
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
