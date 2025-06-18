using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public UpdateCategoryCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Kategori bulunamadı.");

                // Kendi çocuğu olarak seçilmeye çalışılıyor mu kontrol et
                if (request.ParentId == request.Id)
                    return Result<bool>.Failure("Kategori kendi alt kategorisi olamaz.");

                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.ParentId = request.ParentId;
                entity.SortOrder = request.SortOrder;
                entity.IsActive = request.IsActive;
                entity.MetaTitle = string.IsNullOrEmpty(request.MetaTitle) ? request.Name : request.MetaTitle;
                entity.MetaDescription = request.MetaDescription;
                entity.MetaKeywords = request.MetaKeywords;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                // Slug güncelle (isim değiştiyse)
                entity.Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Category", request.Id);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Kategori güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
