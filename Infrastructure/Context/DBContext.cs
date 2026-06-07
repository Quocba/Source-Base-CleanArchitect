using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC2716659579");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Code).HasMaxLength(255);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DepartmentCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Departmen__Creat__1F98B2C1");

                entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.DepartmentLastModifiedByNavigations)
                    .HasForeignKey(d => d.LastModifiedBy)
                    .HasConstraintName("FK__Departmen__LastM__208CD6FA");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC279E2A15F7");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Avatar).HasMaxLength(255);
                entity.Property(e => e.Code).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.PositionId).HasColumnName("PositionID");
                entity.Property(e => e.Status).HasMaxLength(255);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Employees__Creat__2180FB33");

                entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employees__Depar__1CBC4616");

                entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.InverseLastModifiedByNavigation)
                    .HasForeignKey(d => d.LastModifiedBy)
                    .HasConstraintName("FK__Employees__LastM__22751F6C");

                entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employees__Posit__1DB06A4F");

                entity.HasOne(d => d.User).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employees__UserI__1EA48E88");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC2784BBA37E");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Action).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Module).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Position__3214EC2716241E83");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasMany(d => d.Permissions).WithMany(p => p.Positions)
                    .UsingEntity<Dictionary<string, object>>(
                        "PositionPermission",
                        r => r.HasOne<Permission>().WithMany()
                            .HasForeignKey("PermissionId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__PositionP__Permi__42E1EEFE"),
                        l => l.HasOne<Position>().WithMany()
                            .HasForeignKey("PositionId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__PositionP__Posit__41EDCAC5"),
                        j =>
                        {
                            j.HasKey("PositionId", "PermissionId").HasName("PK__Position__8E41F5E9249D5E14");
                            j.ToTable("PositionPermissions");
                            j.IndexerProperty<Guid>("PositionId").HasColumnName("PositionID");
                            j.IndexerProperty<Guid>("PermissionId").HasColumnName("PermissionID");
                        });
            });

            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var managerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Role__3214EC27");

                entity.ToTable("Role");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();

                entity.HasData(
                    new Role { Id = adminRoleId, Name = "Admin" },
                    new Role { Id = managerRoleId, Name = "Manager" }
                );
            });

            var departmentId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasData(new Department
                {
                    Id = departmentId,
                    Name = "Phòng Kỹ Thuật",
                    Code = "PKT",
                    IsDeleted = false
                });
            });

            var positionId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasData(new Position
                {
                    Id = positionId,
                    Name = "Nhân Viên",
                    IsDeleted = false
                });
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasData(new Employee
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    FullName = "Nguyễn Văn A",
                    Code = "NV001",
                    UserId = Guid.Parse("69efb260-d55c-4835-89dc-521e3ceaabee"),
                    DepartmentId = departmentId,
                    PositionId = positionId,
                    Status = "Active"
                });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC274465C354");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.RoleId).HasColumnName("RoleId");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Role_RoleId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
