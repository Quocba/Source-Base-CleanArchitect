using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Department.Command.Create
{
    public class CreateDepartmentCommand : IRequest<ApiResponse<string>>
    {
        [Required(ErrorMessage = "Nhập tên phòng ban")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
