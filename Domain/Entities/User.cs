using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsLock { get; set; }
    public bool? IsUseSoftWare { get; set; } = false;
    public Guid RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;
}
