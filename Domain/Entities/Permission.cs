using System;

namespace Domain.Entities;

public partial class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }
    public string? Action { get; set; }
}
