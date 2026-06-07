using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Department
{
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual Employee? CreatedByNavigation { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Employee? LastModifiedByNavigation { get; set; }
}
