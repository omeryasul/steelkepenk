using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Update
{
    public class UpdateProductStatusCommandHandler : IRequestHandler<UpdateProductStatusCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateProductStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Ürün bulunamadı.");

                entity.Status = request.Status;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ürün durumu güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
