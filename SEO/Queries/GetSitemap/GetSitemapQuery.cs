// GetSitemapQuery.cs - DÜZELTME
using MediatR;

namespace Application.Features.SEO.Queries.GetSitemap
{
    public record GetSitemapQuery() : IRequest<string>;
}