using Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Employee
{
    public class GetMeResponse
    {
        public Guid Id { get; set;}
        public string FullName { get; set; }
        public GenderEnum Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string BankNo { get; set; }
        public string Bank { get; set; }
        public string BankAccountHolder { get; set; }
        public DateTime DBO { get; set; }
    }
}
