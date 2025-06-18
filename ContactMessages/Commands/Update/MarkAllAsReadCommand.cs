using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Update
{
    public record MarkAllAsReadCommand : IRequest<Result<bool>>;

}
