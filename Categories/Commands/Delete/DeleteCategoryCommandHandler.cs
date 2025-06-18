using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Categories
                    .Include(x => x.Children)
                    .Include(x => x.Contents)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Kategori bulunamadı.");

                // Alt kategorileri var mı kontrol et
                if (entity.Children.Any())
                    return Result<bool>.Failure("Bu kategoriye ait alt kategoriler bulunduğu için silinemez.");

                // İçerikleri var mı kontrol et
                if (entity.Contents.Any())
                    return Result<bool>.Failure("Bu kategoriye ait içerikler bulunduğu için silinemez.");

                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Kategori silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}
