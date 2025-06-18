using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.ToggleFeatured
{
    public class ToggleProductFeaturedCommandHandler : IRequestHandler<ToggleProductFeaturedCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ToggleProductFeaturedCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(ToggleProductFeaturedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Ürün bulunamadı.");

                entity.IsFeatured = !entity.IsFeatured;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(entity.IsFeatured);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ürün öne çıkarma durumu güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
