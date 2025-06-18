using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Create
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public CreateProductCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Kategori nesnesini veritabanından çekiyoruz:
                var category = await _context.Categories.FindAsync(request.CategoryId);
                if (category == null)
                    return Result<int>.Failure("Seçilen kategori bulunamadı.");

                var entity = new Product
                {
                    Name = request.Name,
                    Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Product"),
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    Status = request.Status,
                    IsFeatured = request.IsFeatured,
                    SortOrder = request.SortOrder,
                    CategoryId = request.CategoryId,
                    Category = category,  // ✅ Artık doğru şekilde atanıyor
                    Features = request.Features,
                    MainImage = request.MainImage,
                    Specifications = request.Specifications,
                    UsageAreas = request.UsageAreas,
                    Advantages = request.Advantages,
                    DisplayPrice = request.DisplayPrice,
                    PriceNote = request.PriceNote,
                    MetaTitle = string.IsNullOrEmpty(request.MetaTitle) ? request.Name : request.MetaTitle,
                    MetaDescription = request.MetaDescription,
                    MetaKeywords = request.MetaKeywords,
                    OgTitle = request.OgTitle,
                    OgDescription = request.OgDescription,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = _currentUser.UserEmail
                };

                _context.Products.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                if (request.TagIds.Any())
                {
                    var productTags = request.TagIds.Select(tagId => new ProductTag
                    {
                        ProductId = entity.Id,
                        TagId = tagId
                    }).ToList();

                    _context.ProductTags.AddRange(productTags);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                var fullError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Result<int>.Failure($"Ürün oluşturulurken detaylı hata: {fullError}");
            }
        }
    }
}
