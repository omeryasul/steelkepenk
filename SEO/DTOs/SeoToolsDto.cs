using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoToolsDto
    {
        public string? RobotsText { get; set; }
        public bool EnableSitemap { get; set; }
        public string? SitemapUrl { get; set; }
        public string? DefaultOgImage { get; set; }
        public DateTime? LastSitemapGenerated { get; set; }
    }
}
