using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsLock { get; set; } = false;
    public bool IsUseSoftWare { get; set; } = true;
    public Guid RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public virtual Role? Role { get; set; }
}
