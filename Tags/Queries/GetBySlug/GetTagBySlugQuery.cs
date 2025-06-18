using Application.Features.Tags.DTOs;
using MediatR;

namespace Application.Features.Tags.Queries.GetBySlug
{
    public record GetTagBySlugQuery(string Slug) : IRequest<TagDto?>;
}