using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Employee
{
    public class GetEmployeeResponse
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }

        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime? DBO { get; set; }

        public string? Avatar { get; set; }

        public string? Bank { get; set; }

        public string? BankNo { get; set; }

        public string? BankAccountHolder { get; set; }
        public string? Status { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? Department { get; set; }
        public Guid? PositionId { get; set; }
        public string? Position { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid RoleId { get; set; }
        public string Role { get; set; }

    }
}
