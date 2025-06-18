using Application.Common.Interfaces;
using Application.Features.SEO.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetById
{
    public class GetSeoSettingByIdQueryHandler : IRequestHandler<GetSeoSettingByIdQuery, SeoSettingDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetSeoSettingByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SeoSettingDetailDto?> Handle(GetSeoSettingByIdQuery request, CancellationToken cancellationToken)
        {
            var seoSetting = await _context.SeoSettings
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (seoSetting == null)
                return null;

            return new SeoSettingDetailDto
            {
                Id = seoSetting.Id,
                SiteName = seoSetting.SiteName,
                SiteDescription = seoSetting.SiteDescription,
                SiteKeywords = seoSetting.SiteKeywords,
                SiteUrl = seoSetting.SiteUrl,
                GoogleAnalyticsId = seoSetting.GoogleAnalyticsId,
                GoogleTagManagerId = seoSetting.GoogleTagManagerId,
                FacebookPixelId = seoSetting.FacebookPixelId,
                GoogleSearchConsoleCode = seoSetting.GoogleSearchConsoleCode,
                RobotsText = seoSetting.RobotsText,
                EnableSitemap = seoSetting.EnableSitemap,
                DefaultOgImage = seoSetting.DefaultOgImage,
                OrganizationJsonLd = seoSetting.OrganizationJsonLd,
                WebsiteJsonLd = seoSetting.WebsiteJsonLd,
                CreatedDate = seoSetting.CreatedDate,
                UpdatedDate = seoSetting.UpdatedDate,
                CreatedBy = seoSetting.CreatedBy,
                UpdatedBy = seoSetting.UpdatedBy
            };
        }
    }
}
