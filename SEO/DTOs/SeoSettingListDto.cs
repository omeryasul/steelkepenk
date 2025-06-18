using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoSettingListDto
    {
        public int Id { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string SiteDescription { get; set; } = string.Empty;
        public string? SiteUrl { get; set; }
        public bool EnableSitemap { get; set; }
        public bool HasGoogleAnalytics { get; set; }
        public bool HasGoogleTagManager { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
