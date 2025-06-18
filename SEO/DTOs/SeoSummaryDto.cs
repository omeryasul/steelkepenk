using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoSummaryDto
    {
        public int TotalSeoSettings { get; set; }
        public bool HasDefaultSettings { get; set; }
        public bool SitemapEnabled { get; set; }
        public bool AnalyticsConfigured { get; set; }
        public bool SchemaConfigured { get; set; }
        public DateTime? LastUpdated { get; set; }
        public SeoValidationDto ValidationResult { get; set; } = new();
    }
}
