// GetRobotsTextQuery.cs - DÜZELTME
using MediatR;

namespace Application.Features.SEO.Queries.GetRobotsText
{
    public record GetRobotsTextQuery() : IRequest<string>;
}