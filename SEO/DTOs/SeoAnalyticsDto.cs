using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoAnalyticsDto
    {
        public string? GoogleAnalyticsId { get; set; }
        public string? GoogleTagManagerId { get; set; }
        public string? FacebookPixelId { get; set; }
        public string? GoogleSearchConsoleCode { get; set; }
        public bool IsAnalyticsConfigured => !string.IsNullOrEmpty(GoogleAnalyticsId);
        public bool IsTagManagerConfigured => !string.IsNullOrEmpty(GoogleTagManagerId);
        public bool IsFacebookPixelConfigured => !string.IsNullOrEmpty(FacebookPixelId);
        public bool IsSearchConsoleConfigured => !string.IsNullOrEmpty(GoogleSearchConsoleCode);
    }
}
