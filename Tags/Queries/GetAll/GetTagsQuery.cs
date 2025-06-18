using Application.Common.Models;
using Application.Features.Tags.DTOs;
using MediatR;

namespace Application.Features.Tags.Queries.GetAll
{
    public record GetTagsQuery : IRequest<PagedResult<TagDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 50;
        public string? SearchTerm { get; init; }
        public bool? IsActive { get; init; }
        public string? SortBy { get; init; } = "Name";
        public bool SortDescending { get; init; } = false;
    }
}