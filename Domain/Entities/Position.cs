using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Position
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
