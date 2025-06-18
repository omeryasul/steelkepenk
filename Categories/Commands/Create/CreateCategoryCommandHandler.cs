using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public CreateCategoryCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new Category
                {
                    Name = request.Name,
                    Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Category"),
                    Description = request.Description,
                    ParentId = request.ParentId,
                    SortOrder = request.SortOrder,
                    IsActive = request.IsActive,
                    MetaTitle = string.IsNullOrEmpty(request.MetaTitle) ? request.Name : request.MetaTitle,
                    MetaDescription = request.MetaDescription,
                    MetaKeywords = request.MetaKeywords,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = _currentUser.UserEmail
                };

                _context.Categories.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Kategori oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
