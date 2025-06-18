using Application.Common.Models;
using MediatR;

namespace Application.Features.Tags.Commands.Delete
{
    public record DeleteTagCommand(int Id) : IRequest<Result<bool>>;
}