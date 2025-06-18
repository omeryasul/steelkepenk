using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.SEO.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetAll
{
    public class GetSeoSettingsQueryHandler : IRequestHandler<GetSeoSettingsQuery, PagedResult<SeoSettingListDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetSeoSettingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SeoSettingListDto>> Handle(GetSeoSettingsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.SeoSettings.AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x =>
                    x.SiteName.Contains(request.SearchTerm) ||
                    x.SiteDescription.Contains(request.SearchTerm) ||
                    (x.SiteKeywords != null && x.SiteKeywords.Contains(request.SearchTerm)));
            }

            // Sorting
            query = request.SortBy?.ToLower() switch
            {
                "sitename" => request.SortDescending ? query.OrderByDescending(x => x.SiteName) : query.OrderBy(x => x.SiteName),
                "sitedescription" => request.SortDescending ? query.OrderByDescending(x => x.SiteDescription) : query.OrderBy(x => x.SiteDescription),
                _ => request.SortDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var seoSettings = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SeoSettingListDto
                {
                    Id = x.Id,
                    SiteName = x.SiteName,
                    SiteDescription = x.SiteDescription,
                    SiteUrl = x.SiteUrl,
                    EnableSitemap = x.EnableSitemap,
                    HasGoogleAnalytics = !string.IsNullOrEmpty(x.GoogleAnalyticsId),
                    HasGoogleTagManager = !string.IsNullOrEmpty(x.GoogleTagManagerId),
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return PagedResult<SeoSettingListDto>.Create(seoSettings, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
