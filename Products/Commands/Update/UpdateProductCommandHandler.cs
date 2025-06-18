using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Update
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public UpdateProductCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Products
                    .Include(x => x.ProductTags)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Ürün bulunamadı.");

                entity.Name = request.Name;
                entity.ShortDescription = request.ShortDescription;
                entity.Description = request.Description;
                entity.Status = request.Status;
                entity.IsFeatured = request.IsFeatured;
                entity.SortOrder = request.SortOrder;
                entity.CategoryId = request.CategoryId;
                entity.Features = request.Features;
                entity.Specifications = request.Specifications;
                entity.UsageAreas = request.UsageAreas;
                entity.Advantages = request.Advantages;
                entity.DisplayPrice = request.DisplayPrice;
                entity.PriceNote = request.PriceNote;
                entity.MetaTitle = string.IsNullOrEmpty(request.MetaTitle) ? request.Name : request.MetaTitle;
                entity.MetaDescription = request.MetaDescription;
                entity.MetaKeywords = request.MetaKeywords;
                entity.OgTitle = request.OgTitle;
                entity.OgDescription = request.OgDescription;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;
                entity.MainImage = request.MainImage;

                // Slug güncelle
                entity.Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Product", request.Id);

                // Tags güncelle
                _context.ProductTags.RemoveRange(entity.ProductTags);

                if (request.TagIds.Any())
                {
                    var productTags = request.TagIds.Select(tagId => new ProductTag
                    {
                        ProductId = entity.Id,
                        TagId = tagId
                    }).ToList();

                    _context.ProductTags.AddRange(productTags);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ürün güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
