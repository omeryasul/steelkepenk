using System;

namespace Application.Features.SEO.DTOs
{
    public class SeoSettingDetailDto
    {
        public int Id { get; set; }

        // Genel Site SEO Ayarları
        public string SiteName { get; set; } = string.Empty;
        public string SiteDescription { get; set; } = string.Empty;
        public string? SiteKeywords { get; set; }
        public string? SiteUrl { get; set; }

        // Admin panel entegrasyonu için eklenen alanlar
        public string? SiteAuthor { get; set; }

        // Analytics ve Tracking - Admin panel property isimleri
        public string? GoogleAnalytics { get; set; }
        public string? GoogleTagManager { get; set; }
        public string? FacebookPixel { get; set; }

        // Backward compatibility için mevcut property'ler
        public string? GoogleAnalyticsId
        {
            get => GoogleAnalytics;
            set => GoogleAnalytics = value;
        }
        public string? GoogleTagManagerId
        {
            get => GoogleTagManager;
            set => GoogleTagManager = value;
        }
        public string? FacebookPixelId
        {
            get => FacebookPixel;
            set => FacebookPixel = value;
        }

        public string? GoogleSearchConsoleCode { get; set; }

        // SEO Araçları
        public string? RobotsText { get; set; }
        public bool EnableSitemap { get; set; }
        public string? DefaultOgImage { get; set; }

        // Schema.org Yapılandırması
        public string? OrganizationJsonLd { get; set; }
        public string? WebsiteJsonLd { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}