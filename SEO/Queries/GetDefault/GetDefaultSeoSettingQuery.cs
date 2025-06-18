// GetDefaultSeoSettingQuery.cs - DÜZELTME
using Application.Features.SEO.DTOs;
using MediatR;

namespace Application.Features.SEO.Queries.GetDefault
{
    public record GetDefaultSeoSettingQuery : IRequest<SeoSettingDetailDto?>;
}