using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Possition.Command.Create
{
    public class CreatePositionCommand : IRequest<ApiResponse<string>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
