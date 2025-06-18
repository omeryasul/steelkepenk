using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Create
{
    public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;

        public CreateContentCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService)
        {
            _context = context;
            _slugService = slugService;
        }

        public async Task<Result<int>> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Slug oluştur veya kontrol et
                var slug = string.IsNullOrEmpty(request.Slug)
                    ? _slugService.GenerateSlug(request.Title)
                    : _slugService.GenerateSlug(request.Slug);

                // Slug benzersizlik kontrolü
                var existingContent = await _context.Contents
                    .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

                if (existingContent != null)
                {
                    slug = await _slugService.EnsureUniqueSlugAsync(slug, "Content");
                }

                // Category kontrolü
                var categoryExists = await _context.Categories
                    .AnyAsync(x => x.Id == request.CategoryId && x.IsActive, cancellationToken);

                if (!categoryExists)
                {
                    return Result<int>.Failure("Belirtilen kategori bulunamadı veya aktif değil.");
                }

                // Content entity oluştur
                var content = new Content
                {
                    Title = request.Title.Trim(),
                    Slug = slug,
                    Summary = request.Summary.Trim(),
                    Body = request.Body.Trim(),
                    FeaturedImage = request.FeaturedImage?.Trim(),
                    Type = request.Type,
                    Status = request.Status,
                    CategoryId = request.CategoryId,
                    IsFeatured = request.IsFeatured,
                    SortOrder = request.SortOrder,
                    MetaTitle = string.IsNullOrEmpty(request.MetaTitle) ? request.Title : request.MetaTitle.Trim(),
                    MetaDescription = string.IsNullOrEmpty(request.MetaDescription) ? request.Summary : request.MetaDescription.Trim(),
                    MetaKeywords = request.MetaKeywords?.Trim(),
                    OgTitle = request.OgTitle?.Trim(),
                    OgDescription = request.OgDescription?.Trim(),
                    OgImage = request.OgImage?.Trim(),
                    CanonicalUrl = request.CanonicalUrl?.Trim(),
                    NoIndex = request.NoIndex,
                    NoFollow = request.NoFollow,
                    CreatedDate = DateTime.UtcNow
                };

                // Published date logic
                if (request.Status == ContentStatus.Published)
                {
                    content.PublishedDate = request.PublishedDate ?? DateTime.UtcNow;
                }

                _context.Contents.Add(content);
                await _context.SaveChangesAsync(cancellationToken);

                // Tags işlemi
                if (request.Tags?.Any() == true)
                {
                    await ProcessTagsAsync(content, request.Tags, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result<int>.Success(content.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"İçerik oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        private async Task ProcessTagsAsync(Content content, List<string> tagNames, CancellationToken cancellationToken)
        {
            if (tagNames == null || !tagNames.Any())
                return;

            var cleanTagNames = tagNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            foreach (var tagName in cleanTagNames)
            {
                // Tag var mı kontrol et
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == tagName.ToLower(), cancellationToken);

                if (tag == null)
                {
                    // Yeni tag oluştur
                    tag = new Tag
                    {
                        Name = tagName,
                        Slug = _slugService.GenerateSlug(tagName),
                        IsActive = true,
                    };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // ContentTag ilişkisi oluştur
                var contentTag = new ContentTag
                {
                    ContentId = content.Id,
                    TagId = tag.Id
                };

                _context.ContentTags.Add(contentTag);
            }
        }
    }
}