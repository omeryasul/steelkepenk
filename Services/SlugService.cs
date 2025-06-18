// SlugService.cs - Persistence/Services
using Application.Common.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace Persistence.Services
{
    public class SlugService : ISlugService
    {
        private readonly ApplicationDbContext _context;

        public SlugService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueSlugAsync(string input, string entityType, int? excludeId = null)
        {
            var baseSlug = GenerateSlug(input);
            return await EnsureUniqueSlugAsync(baseSlug, entityType, excludeId);
        }

        public async Task<string> EnsureUniqueSlugAsync(string slug, string entityType, int? excludeId = null)
        {
            var baseSlug = slug;
            var counter = 1;

            while (!await IsSlugUniqueAsync(slug, entityType, excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public string EnsureUniqueSlug(string slug, string entityType, int? excludeId = null)
        {
            return EnsureUniqueSlugAsync(slug, entityType, excludeId).Result;
        }

        public string GenerateSlug(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Türkçe karakterleri dönüştür
            input = ConvertTurkishCharacters(input);

            // Küçük harfe çevir
            input = input.ToLowerInvariant();

            // Özel karakterleri kaldır, sadece harf, rakam ve tire bırak
            input = Regex.Replace(input, @"[^a-z0-9\s-]", "");

            // Birden fazla boşluğu tek boşlukla değiştir
            input = Regex.Replace(input, @"\s+", " ").Trim();

            // Boşlukları tire ile değiştir
            input = Regex.Replace(input, @"\s", "-");

            // Birden fazla tireyi tek tire ile değiştir
            input = Regex.Replace(input, @"-+", "-");

            // Başındaki ve sonundaki tireleri kaldır
            input = input.Trim('-');

            return input;
        }

        public bool IsValidSlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return false;

            // Slug sadece küçük harf, rakam ve tire içermeli
            return Regex.IsMatch(slug, @"^[a-z0-9-]+$") &&
                   !slug.StartsWith('-') &&
                   !slug.EndsWith('-') &&
                   !slug.Contains("--");
        }

        private string ConvertTurkishCharacters(string input)
        {
            var turkishChars = new Dictionary<char, char>
            {
                {'ç', 'c'}, {'Ç', 'C'},
                {'ğ', 'g'}, {'Ğ', 'G'},
                {'ı', 'i'}, {'I', 'I'},
                {'İ', 'I'}, {'i', 'i'},
                {'ö', 'o'}, {'Ö', 'O'},
                {'ş', 's'}, {'Ş', 'S'},
                {'ü', 'u'}, {'Ü', 'U'}
            };

            var result = new StringBuilder();
            foreach (char c in input)
            {
                result.Append(turkishChars.TryGetValue(c, out var replacement) ? replacement : c);
            }

            return result.ToString();
        }

        private async Task<bool> IsSlugUniqueAsync(string slug, string entityType, int? excludeId)
        {
            return entityType.ToLower() switch
            {
                "category" => await IsSlugUniqueInCategoryAsync(slug, excludeId),
                "content" => await IsSlugUniqueInContentAsync(slug, excludeId),
                "product" => await IsSlugUniqueInProductAsync(slug, excludeId),
                "productcategory" => await IsSlugUniqueInProductCategoryAsync(slug, excludeId),
                "page" => await IsSlugUniqueInPageAsync(slug, excludeId),
                _ => true
            };
        }

        private async Task<bool> IsSlugUniqueInCategoryAsync(string slug, int? excludeId)
        {
            var query = _context.Categories.Where(x => x.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        private async Task<bool> IsSlugUniqueInContentAsync(string slug, int? excludeId)
        {
            var query = _context.Contents.Where(x => x.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        private async Task<bool> IsSlugUniqueInProductAsync(string slug, int? excludeId)
        {
            var query = _context.Products.Where(x => x.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        private async Task<bool> IsSlugUniqueInProductCategoryAsync(string slug, int? excludeId)
        {
            var query = _context.Categories.Where(x => x.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        private async Task<bool> IsSlugUniqueInPageAsync(string slug, int? excludeId)
        {
            var query = _context.Pages.Where(x => x.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}