using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Contents.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Queries.GetById
{
    public class GetContentByIdQueryHandler : IRequestHandler<GetContentByIdQuery, Result<ContentDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetContentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ContentDetailDto>> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var content = await _context.Contents
                    .Include(x => x.Category)
                    .Include(x => x.ContentTags)
                        .ThenInclude(x => x.Tag)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (content == null)
                {
                    return Result<ContentDetailDto>.Failure("İçerik bulunamadı.");
                }

                var contentDto = new ContentDetailDto
                {
                    Id = content.Id,
                    Title = content.Title,
                    Slug = content.Slug,
                    Summary = content.Summary,
                    Body = content.Body,
                    FeaturedImage = content.FeaturedImage,
                    CreatedDate = content.CreatedDate,
                    UpdatedDate = content.UpdatedDate,
                    Status = content.Status,
                    Type = content.Type,
                    ViewCount = content.ViewCount,
                    IsFeatured = content.IsFeatured,
                    SortOrder = content.SortOrder,
                    CategoryId = content.CategoryId,
                    CategoryName = content.Category?.Name,
                    CategorySlug = content.Category?.Slug,
                    MetaTitle = content.MetaTitle,
                    MetaDescription = content.MetaDescription,
                    MetaKeywords = content.MetaKeywords,
                    OgTitle = content.OgTitle,
                    OgDescription = content.OgDescription,
                    OgImage = content.OgImage,
                    Tags = content.ContentTags.Select(ct => new ContentTagDto
                    {
                        Id = ct.Tag.Id,
                        Name = ct.Tag.Name,
                        Slug = ct.Tag.Slug
                    }).ToList()
                };

                // Navigation bilgileri (önceki/sonraki içerik)
                if (request.IncludeNavigation)
                {
                    var previousContent = await _context.Contents
                        .Where(x => x.Id < request.Id && x.Status == Domain.Enums.ContentStatus.Published && x.CategoryId == content.CategoryId)
                        .OrderByDescending(x => x.Id)
                        .Select(x => new ContentNavigationDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Slug = x.Slug
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    var nextContent = await _context.Contents
                        .Where(x => x.Id > request.Id && x.Status == Domain.Enums.ContentStatus.Published && x.CategoryId == content.CategoryId)
                        .OrderBy(x => x.Id)
                        .Select(x => new ContentNavigationDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Slug = x.Slug
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    contentDto.PreviousContent = previousContent;
                    contentDto.NextContent = nextContent;
                }

                // View count artır
                if (request.IncreaseViewCount)
                {
                    content.ViewCount++;
                    await _context.SaveChangesAsync(cancellationToken);
                    contentDto.ViewCount = content.ViewCount;
                }

                return Result<ContentDetailDto>.Success(contentDto);
            }
            catch (Exception ex)
            {
                return Result<ContentDetailDto>.Failure($"İçerik detayı getirilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
