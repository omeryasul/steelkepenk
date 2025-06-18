using Application.Common.Models;
using MediatR;

namespace Application.Features.Tags.Commands.Update
{
    public record UpdateTagCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Color { get; init; }
        public bool IsActive { get; init; } = true;
    }
}