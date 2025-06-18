// GetSeoSettingByIdQuery.cs - DÜZELTME
using Application.Features.SEO.DTOs;
using MediatR;

namespace Application.Features.SEO.Queries.GetById
{
    public record GetSeoSettingByIdQuery(int Id) : IRequest<SeoSettingDetailDto?>;
}