using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class SeoMetaData
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Keywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public string? OgImage { get; set; }
        public string? OgType { get; set; } = "website";
        public string? OgUrl { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? JsonLd { get; set; }
        public string? TwitterCard { get; set; } = "summary_large_image";
        public string? TwitterTitle { get; set; }
        public string? TwitterDescription { get; set; }
        public string? TwitterImage { get; set; }
    }
}
