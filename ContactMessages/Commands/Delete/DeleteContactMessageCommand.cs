using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Delete
{
    public record DeleteContactMessageCommand(int Id) : IRequest<Result<bool>>;

}
