using Application.Common.Models;
using MediatR;

namespace Application.Features.Tags.Commands.Create
{
    public record CreateTagCommand : IRequest<Result<int>>
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Color { get; init; } // Hex color: #FF5733
        public bool IsActive { get; init; } = true;
    }
}