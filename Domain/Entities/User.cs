using Domain.Entities.Base;
using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Domain.Entities;

[Table("Users")]
public class User : BaseEntity<Guid>
{
    public string Code { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool IsLock { get; set; } = false;
    public string Phone { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
    public long StorageCapacity { get; set; } = 5368709120;
    public ERole Role { get; set; }
    
}
