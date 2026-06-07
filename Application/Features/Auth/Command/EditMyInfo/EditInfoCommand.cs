using Discord.Net;
using Domain.Entities.Enum;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.EditMyInfo
{
    public class EditInfoCommand : IRequest<ApiResponse<string>>
    {
        public string? FullName { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? BankNo { get; set; }
        public string? Bank { get; set; }
        public string? BankAccountHolder { get; set; }
        public DateTime DBO { get; set; }
    }
}
