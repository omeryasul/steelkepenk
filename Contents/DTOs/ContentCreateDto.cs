using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.DTOs
{
    public class ContentCreateDto
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Slug en fazla 250 karakter olabilir.")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Özet alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Özet en fazla 500 karakter olabilir.")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        public string Body { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir resim URL'si giriniz.")]
        public string? FeaturedImage { get; set; }

        [Required(ErrorMessage = "İçerik türü seçilmelidir.")]
        public ContentType Type { get; set; }

        public ContentStatus Status { get; set; } = ContentStatus.Draft;

        [Required(ErrorMessage = "Kategori seçilmelidir.")]
        public int CategoryId { get; set; }

        public bool IsFeatured { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Sıralama değeri 0 veya pozitif olmalıdır.")]
        public int SortOrder { get; set; }

        // SEO Fields
        [StringLength(160, ErrorMessage = "Meta başlık en fazla 160 karakter olabilir.")]
        public string MetaTitle { get; set; } = string.Empty;

        [StringLength(320, ErrorMessage = "Meta açıklama en fazla 320 karakter olabilir.")]
        public string MetaDescription { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Meta anahtar kelimeler en fazla 500 karakter olabilir.")]
        public string? MetaKeywords { get; set; }

        [StringLength(160, ErrorMessage = "OG başlık en fazla 160 karakter olabilir.")]
        public string? OgTitle { get; set; }

        [StringLength(320, ErrorMessage = "OG açıklama en fazla 320 karakter olabilir.")]
        public string? OgDescription { get; set; }

        [Url(ErrorMessage = "Geçerli bir OG resim URL'si giriniz.")]
        public string? OgImage { get; set; }

        // Tags (comma separated string or list)
        public List<string> Tags { get; set; } = new();
    }
}
