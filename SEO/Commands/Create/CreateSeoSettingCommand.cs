using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Create
{
    public record CreateSeoSettingCommand : IRequest<Result<int>>
    {
        public string SiteName { get; init; } = string.Empty;
        public string SiteDescription { get; init; } = string.Empty;
        public string? SiteKeywords { get; init; }
        public string? SiteUrl { get; init; }
        public string? GoogleAnalyticsId { get; init; }
        public string? GoogleTagManagerId { get; init; }
        public string? FacebookPixelId { get; init; }
        public string? GoogleSearchConsoleCode { get; init; }
        public string? RobotsText { get; init; }
        public bool EnableSitemap { get; init; } = true;
        public string? DefaultOgImage { get; init; }
        public string? OrganizationJsonLd { get; init; }
        public string? WebsiteJsonLd { get; init; }
    }
}
