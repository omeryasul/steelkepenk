// GetPageSeoDataQuery.cs - DÜZELTME
using Application.Common.Models;
using MediatR;

namespace Application.Features.SEO.Queries.GetPageSeoData
{
    public record GetPageSeoDataQuery(string Path) : IRequest<SeoMetaData?>;
}