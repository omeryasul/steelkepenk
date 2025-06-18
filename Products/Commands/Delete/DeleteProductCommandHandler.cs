using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Delete
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Products
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductTags)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Ürün bulunamadı.");

                // İlişkili verileri sil
                _context.ProductImages.RemoveRange(entity.ProductImages);
                _context.ProductTags.RemoveRange(entity.ProductTags);
                _context.Products.Remove(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ürün silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}
