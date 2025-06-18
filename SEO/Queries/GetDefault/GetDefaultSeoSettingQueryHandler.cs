using Application.Common.Interfaces;
using Application.Features.SEO.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetDefault
{
    public class GetDefaultSeoSettingQueryHandler : IRequestHandler<GetDefaultSeoSettingQuery, SeoSettingDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetDefaultSeoSettingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SeoSettingDetailDto?> Handle(GetDefaultSeoSettingQuery request, CancellationToken cancellationToken)
        {
            // İlk SEO ayarını varsayılan olarak kabul ediyoruz
            var seoSetting = await _context.SeoSettings
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

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
