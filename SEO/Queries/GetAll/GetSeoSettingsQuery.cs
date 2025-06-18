using Application.Common.Models;
using Application.Features.SEO.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetAll
{
    public record GetSeoSettingsQuery : IRequest<PagedResult<SeoSettingListDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public string? SortBy { get; init; } = "CreatedDate";
        public bool SortDescending { get; init; } = true;
    }
}
