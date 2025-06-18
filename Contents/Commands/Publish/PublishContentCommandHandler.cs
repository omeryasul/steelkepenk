using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Publish
{
    public class PublishContentCommandHandler : IRequestHandler<PublishContentCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public PublishContentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(PublishContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var content = await _context.Contents
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (content == null)
                {
                    return Result<bool>.Failure("İçerik bulunamadı.");
                }

                if (content.Status == ContentStatus.Published)
                {
                    return Result<bool>.Failure("İçerik zaten yayınlanmış durumda.");
                }

                // Category kontrolü
                var categoryExists = await _context.Categories
                    .AnyAsync(x => x.Id == content.CategoryId && x.IsActive, cancellationToken);

                if (!categoryExists)
                {
                    return Result<bool>.Failure("İçeriğin kategorisi aktif değil. Önce kategoriyi aktifleştirin.");
                }

                content.Status = ContentStatus.Published;
                content.UpdatedDate = DateTime.UtcNow;
                content.UpdatedBy = _currentUser.UserEmail;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İçerik yayınlanırken hata oluştu: {ex.Message}");
            }
        }
    }
}
