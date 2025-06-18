using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Publish
{
    public record PublishContentCommand(int Id) : IRequest<Result<bool>>;

}
