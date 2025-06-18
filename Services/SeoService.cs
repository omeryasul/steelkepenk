using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence.Services
{
    public class SeoService : ISeoService
    {
        private readonly ApplicationDbContext _context;
        private static string? _cachedSitemap;
        private static DateTime _lastSitemapGeneration = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

        public SeoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateSitemapAsync()
        {
            // Cache kontrolü
            if (_cachedSitemap != null && DateTime.UtcNow - _lastSitemapGeneration < _cacheExpiration)
            {
                return _cachedSitemap;
            }

            var seoSettings = await _context.SeoSettings.FirstOrDefaultAsync();
            if (seoSettings?.EnableSitemap != true || string.IsNullOrEmpty(seoSettings.SiteUrl))
            {
                return GenerateEmptySitemap();
            }

            var baseUrl = seoSettings.SiteUrl.TrimEnd('/');
            var sitemap = new StringBuilder();

            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Ana sayfa
            sitemap.AppendLine($"  <url>");
            sitemap.AppendLine($"    <loc>{baseUrl}/</loc>");
            sitemap.AppendLine($"    <changefreq>daily</changefreq>");
            sitemap.AppendLine($"    <priority>1.0</priority>");
            sitemap.AppendLine($"  </url>");

            // İçerikler
            var contents = await _context.Contents
                .Where(x => x.Status == Domain.Enums.ContentStatus.Published)
                .Select(x => new { x.Slug, x.UpdatedDate, x.CreatedDate })
                .ToListAsync();

            foreach (var content in contents)
            {
                var lastMod = content.UpdatedDate ?? content.CreatedDate;
                sitemap.AppendLine($"  <url>");
                sitemap.AppendLine($"    <loc>{baseUrl}/icerik/{content.Slug}</loc>");
                sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine($"    <changefreq>weekly</changefreq>");
                sitemap.AppendLine($"    <priority>0.8</priority>");
                sitemap.AppendLine($"  </url>");
            }

            // Ürünler
            var products = await _context.Products
                .Where(x => x.Status == Domain.Enums.ProductStatus.Active)
                .Select(x => new { x.Slug, x.UpdatedDate, x.CreatedDate })
                .ToListAsync();

            foreach (var product in products)
            {
                var lastMod = product.UpdatedDate ?? product.CreatedDate;
                sitemap.AppendLine($"  <url>");
                sitemap.AppendLine($"    <loc>{baseUrl}/urun/{product.Slug}</loc>");
                sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine($"    <changefreq>weekly</changefreq>");
                sitemap.AppendLine($"    <priority>0.9</priority>");
                sitemap.AppendLine($"  </url>");
            }

            // Kategoriler
            var categories = await _context.Categories
                .Where(x => x.IsActive)
                .Select(x => new { x.Slug, x.UpdatedDate, x.CreatedDate })
                .ToListAsync();

            foreach (var category in categories)
            {
                var lastMod = category.UpdatedDate ?? category.CreatedDate;
                sitemap.AppendLine($"  <url>");
                sitemap.AppendLine($"    <loc>{baseUrl}/kategori/{category.Slug}</loc>");
                sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine($"    <changefreq>weekly</changefreq>");
                sitemap.AppendLine($"    <priority>0.7</priority>");
                sitemap.AppendLine($"  </url>");
            }

            // Ürün kategorileri
            var productCategories = await _context.Categories
                .Where(x => x.IsActive)
                .Select(x => new { x.Slug, x.UpdatedDate, x.CreatedDate })
                .ToListAsync();

            foreach (var category in productCategories)
            {
                var lastMod = category.UpdatedDate ?? category.CreatedDate;
                sitemap.AppendLine($"  <url>");
                sitemap.AppendLine($"    <loc>{baseUrl}/urun-kategori/{category.Slug}</loc>");
                sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine($"    <changefreq>weekly</changefreq>");
                sitemap.AppendLine($"    <priority>0.7</priority>");
                sitemap.AppendLine($"  </url>");
            }

            // Sayfalar
            var pages = await _context.Pages
                .Where(x => x.Status == Domain.Enums.ContentStatus.Published)
                .Select(x => new { x.Slug, x.UpdatedDate, x.CreatedDate })
                .ToListAsync();

            foreach (var page in pages)
            {
                var lastMod = page.UpdatedDate ?? page.CreatedDate;
                sitemap.AppendLine($"  <url>");
                sitemap.AppendLine($"    <loc>{baseUrl}/{page.Slug}</loc>");
                sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine($"    <changefreq>monthly</changefreq>");
                sitemap.AppendLine($"    <priority>0.6</priority>");
                sitemap.AppendLine($"  </url>");
            }

            sitemap.AppendLine("</urlset>");

            _cachedSitemap = sitemap.ToString();
            _lastSitemapGeneration = DateTime.UtcNow;

            return _cachedSitemap;
        }

        public async Task<string> GenerateRobotsTextAsync()
        {
            var seoSettings = await _context.SeoSettings.FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(seoSettings?.RobotsText))
            {
                return seoSettings.RobotsText;
            }

            var robots = new StringBuilder();
            robots.AppendLine("User-agent: *");
            robots.AppendLine("Disallow:");
            robots.AppendLine("Disallow: /admin/");
            robots.AppendLine("Disallow: /api/");

            if (!string.IsNullOrEmpty(seoSettings?.SiteUrl))
            {
                robots.AppendLine($"Sitemap: {seoSettings.SiteUrl.TrimEnd('/')}/sitemap.xml");
            }

            return robots.ToString();
        }

        public async Task<SeoMetaData> GetPageSeoDataAsync(string path)
        {
            var seoData = new SeoMetaData();
            var seoSettings = await _context.SeoSettings.FirstOrDefaultAsync();

            // Varsayılan değerler
            if (seoSettings != null)
            {
                seoData.Title = seoSettings.SiteName;
                seoData.Description = seoSettings.SiteDescription;
                seoData.Keywords = seoSettings.SiteKeywords;
                seoData.OgImage = seoSettings.DefaultOgImage;
                seoData.OgUrl = !string.IsNullOrEmpty(seoSettings.SiteUrl) ? $"{seoSettings.SiteUrl.TrimEnd('/')}{path}" : null;
            }

            // Path'e göre özel SEO verileri
            if (path.StartsWith("/icerik/"))
            {
                var slug = path.Replace("/icerik/", "");
                var content = await _context.Contents
                    .FirstOrDefaultAsync(x => x.Slug == slug && x.Status == Domain.Enums.ContentStatus.Published);

                if (content != null)
                {
                    seoData.Title = !string.IsNullOrEmpty(content.MetaTitle) ? content.MetaTitle : content.Title;
                    seoData.Description = !string.IsNullOrEmpty(content.MetaDescription) ? content.MetaDescription : content.Summary ?? "";
                    seoData.Keywords = content.MetaKeywords;
                    seoData.OgTitle = content.OgTitle ?? seoData.Title;
                    seoData.OgDescription = content.OgDescription ?? seoData.Description;
                    seoData.OgImage = content.OgImage ?? content.FeaturedImage ?? seoData.OgImage;
                }
            }
            else if (path.StartsWith("/urun/"))
            {
                var slug = path.Replace("/urun/", "");
                var product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Slug == slug && x.Status == Domain.Enums.ProductStatus.Active);

                if (product != null)
                {
                    seoData.Title = !string.IsNullOrEmpty(product.MetaTitle) ? product.MetaTitle : product.Name;
                    seoData.Description = !string.IsNullOrEmpty(product.MetaDescription) ? product.MetaDescription : product.ShortDescription ?? "";
                    seoData.Keywords = product.MetaKeywords;
                    seoData.OgTitle = product.OgTitle ?? seoData.Title;
                    seoData.OgDescription = product.OgDescription ?? seoData.Description;
                    seoData.OgImage = product.OgImage ?? product.MainImage ?? seoData.OgImage;
                }
            }

            return seoData;
        }

        public async Task InvalidateSitemapCacheAsync()
        {
            _cachedSitemap = null;
            _lastSitemapGeneration = DateTime.MinValue;
            await Task.CompletedTask;
        }

        public string GenerateStructuredData(object data, string type)
        {
            var structuredData = new
            {
                context = "https://schema.org",
                type = type,
                data = data
            };

            return JsonSerializer.Serialize(structuredData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        private static string GenerateEmptySitemap()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"></urlset>";
        }
    }
}