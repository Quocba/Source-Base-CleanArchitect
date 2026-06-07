using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public partial class Employee
{
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? FullName { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }
    public DateTime? DBO { get; set; } = DateTime.MinValue;
    public string? Avatar { get; set; }

    public string? Bank { get; set; }

    public string? BankNo { get; set; }

    public string? BankAccountHolder { get; set; }

    public string? Status { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public Guid UserId { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }

    public virtual Employee? CreatedByNavigation { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Department> DepartmentCreatedByNavigations { get; set; } = new List<Department>();

    public virtual ICollection<Department> DepartmentLastModifiedByNavigations { get; set; } = new List<Department>();

    public virtual ICollection<Employee> InverseCreatedByNavigation { get; set; } = new List<Employee>();

    public virtual ICollection<Employee> InverseLastModifiedByNavigation { get; set; } = new List<Employee>();

    public virtual Employee? LastModifiedByNavigation { get; set; }

    public virtual Position Position { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
