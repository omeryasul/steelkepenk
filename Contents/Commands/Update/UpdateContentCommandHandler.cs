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
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Update
{
    public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;

        public UpdateContentCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService)
        {
            _context = context;
            _slugService = slugService;
        }

        public async Task<Result<bool>> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var content = await _context.Contents
                    .Include(x => x.ContentTags)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (content == null)
                {
                    return Result<bool>.Failure("İçerik bulunamadı.");
                }

                // Slug işlemi
                var newSlug = string.IsNullOrEmpty(request.Slug)
                    ? _slugService.GenerateSlug(request.Title)
                    : _slugService.GenerateSlug(request.Slug);

                // Slug değişti mi ve benzersiz mi kontrol et
                if (content.Slug != newSlug)
                {
                    var existingContent = await _context.Contents
                        .FirstOrDefaultAsync(x => x.Slug == newSlug && x.Id != request.Id, cancellationToken);

                    if (existingContent != null)
                    {
                        newSlug = await _slugService.EnsureUniqueSlugAsync(newSlug, "Content", request.Id);
                    }
                }

                // Category kontrolü
                if (request.CategoryId.HasValue)
                {
                    var categoryExists = await _context.Categories
                        .AnyAsync(x => x.Id == request.CategoryId.Value && x.IsActive, cancellationToken);

                    if (!categoryExists)
                    {
                        return Result<bool>.Failure("Belirtilen kategori bulunamadı veya aktif değil.");
                    }
                }

                // Content güncelle
                content.Title = request.Title.Trim();
                content.Slug = newSlug;
                content.Summary = request.Summary.Trim();
                content.Body = request.Content.Trim(); // Body property'sini kullanıyoruz
                content.ImageUrl = request.ImageUrl?.Trim();
                content.Type = request.Type;
                content.Status = request.Status;
                content.CategoryId = request.CategoryId ?? content.CategoryId; // Null check
                content.IsFeatured = request.IsFeatured;
                content.SortOrder = request.SortOrder;
                content.UpdatedDate = DateTime.UtcNow; // UpdatedDate kullanıyoruz

                // Published date logic
                if (request.Status == ContentStatus.Published && content.PublishedDate == null)
                {
                    content.PublishedDate = request.PublishedDate ?? DateTime.UtcNow;
                }
                else if (request.Status == ContentStatus.Published)
                {
                    content.PublishedDate = request.PublishedDate ?? content.PublishedDate;
                }
                else if (request.Status != ContentStatus.Published)
                {
                    content.PublishedDate = null;
                }

                // SEO fields
                content.MetaTitle = request.MetaTitle?.Trim() ?? content.MetaTitle;
                content.MetaDescription = request.MetaDescription?.Trim() ?? content.MetaDescription;
                content.MetaKeywords = request.MetaKeywords?.Trim();
                content.CanonicalUrl = request.CanonicalUrl?.Trim();
                content.NoIndex = request.NoIndex ?? content.NoIndex;
                content.NoFollow = request.NoFollow ?? content.NoFollow;

                // Tags güncelle
                if (request.Tags != null)
                {
                    await UpdateTagsAsync(content, request.Tags, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İçerik güncellenirken hata oluştu: {ex.Message}");
            }
        }

        private async Task UpdateTagsAsync(Content content, List<string> newTagNames, CancellationToken cancellationToken)
        {
            // Mevcut tag ilişkilerini sil
            _context.ContentTags.RemoveRange(content.ContentTags);

            var cleanTagNames = newTagNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            foreach (var tagName in cleanTagNames)
            {
                // Tag var mı kontrol et
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(x => x.Name == tagName, cancellationToken);

                if (tag == null)
                {
                    // Yeni tag oluştur
                    tag = new Tag
                    {
                        Name = tagName,
                        Slug = _slugService.GenerateSlug(tagName),
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