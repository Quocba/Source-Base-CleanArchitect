using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Permission
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Module { get; set; }

    public string? Action { get; set; }

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
}
