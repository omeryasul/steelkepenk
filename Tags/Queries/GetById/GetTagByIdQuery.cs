using Application.Features.Tags.DTOs;
using MediatR;

namespace Application.Features.Tags.Queries.GetById
{
    public record GetTagByIdQuery(int Id) : IRequest<TagDto?>;
}