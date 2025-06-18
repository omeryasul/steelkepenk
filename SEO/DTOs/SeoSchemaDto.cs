using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.DTOs
{
    public class SeoSchemaDto
    {
        public string? OrganizationJsonLd { get; set; }
        public string? WebsiteJsonLd { get; set; }
        public bool HasOrganizationSchema => !string.IsNullOrEmpty(OrganizationJsonLd);
        public bool HasWebsiteSchema => !string.IsNullOrEmpty(WebsiteJsonLd);
    }
}
