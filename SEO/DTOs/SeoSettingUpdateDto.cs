using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoSettingUpdateDto
    {
        public int Id { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string SiteDescription { get; set; } = string.Empty;
        public string? SiteKeywords { get; set; }
        public string? SiteUrl { get; set; }
        public string? GoogleAnalyticsId { get; set; }
        public string? GoogleTagManagerId { get; set; }
        public string? FacebookPixelId { get; set; }
        public string? GoogleSearchConsoleCode { get; set; }
        public string? RobotsText { get; set; }
        public bool EnableSitemap { get; set; } = true;
        public string? DefaultOgImage { get; set; }
        public string? OrganizationJsonLd { get; set; }
        public string? WebsiteJsonLd { get; set; }
    }
}
