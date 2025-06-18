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

namespace Application.Features.Contents.Queries.GetAll
{
    public class GetContentsQueryHandler : IRequestHandler<GetContentsQuery, Result<PagedResult<ContentListDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetContentsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<ContentListDto>>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Contents
                    .Include(x => x.Category)
                    .Include(x => x.ContentTags)
                        .ThenInclude(x => x.Tag)
                    .AsQueryable();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    query = query.Where(x =>
                        x.Title.ToLower().Contains(searchTerm) ||
                        x.Summary.ToLower().Contains(searchTerm) ||
                        x.Body.ToLower().Contains(searchTerm));
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }

                if (request.Type.HasValue)
                {
                    query = query.Where(x => x.Type == request.Type.Value);
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(x => x.CategoryId == request.CategoryId.Value);
                }

                if (request.IsFeatured.HasValue)
                {
                    query = query.Where(x => x.IsFeatured == request.IsFeatured.Value);
                }

                // Sıralama - CreatedDate ve UpdatedDate kullanıyoruz
                query = request.SortBy?.ToLower() switch
                {
                    "title" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.Title)
                        : query.OrderBy(x => x.Title),
                    "createddate" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.CreatedDate)  // CreatedDate kullanıyoruz
                        : query.OrderBy(x => x.CreatedDate),
                    "updateddate" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.UpdatedDate)  // UpdatedDate kullanıyoruz
                        : query.OrderBy(x => x.UpdatedDate),
                    "viewcount" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.ViewCount)
                        : query.OrderBy(x => x.ViewCount),
                    "sortorder" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.SortOrder)
                        : query.OrderBy(x => x.SortOrder),
                    _ => query.OrderByDescending(x => x.CreatedDate)  // CreatedDate kullanıyoruz
                };

                // Toplam kayıt sayısı
                var totalCount = await query.CountAsync(cancellationToken);

                // Sayfalama
                var contents = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new ContentListDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        Summary = x.Summary,
                        FeaturedImage = x.FeaturedImage,
                        CreatedDate = x.CreatedDate,      // Entity'den CreatedDate alıyoruz
                        UpdatedDate = x.UpdatedDate,      // Entity'den UpdatedDate alıyoruz
                        Status = x.Status,
                        Type = x.Type,
                        ViewCount = x.ViewCount,
                        IsFeatured = x.IsFeatured,
                        SortOrder = x.SortOrder,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        CategorySlug = x.Category.Slug,
                        MetaTitle = x.MetaTitle,
                        MetaDescription = x.MetaDescription,
                        Tags = x.ContentTags.Select(ct => ct.Tag.Name).ToList()
                    })
                    .ToListAsync(cancellationToken);

                // PagedResult constructor'ını doğru kullanıyoruz
                var pagedResult = new PagedResult<ContentListDto>(
                    contents,           // items
                    totalCount,         // count
                    request.Page,       // pageNumber
                    request.PageSize    // pageSize
                );

                return Result<PagedResult<ContentListDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ContentListDto>>.Failure($"İçerikler listelenirken hata oluştu: {ex.Message}"); // Failure kullanıyoruz
            }
        }
    }
}