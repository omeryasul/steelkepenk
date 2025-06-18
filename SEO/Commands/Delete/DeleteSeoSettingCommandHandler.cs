using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Delete
{
    public class DeleteSeoSettingCommandHandler : IRequestHandler<DeleteSeoSettingCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSeoSettingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteSeoSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.SeoSettings
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("SEO ayarı bulunamadı.");

                _context.SeoSettings.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"SEO ayarı silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}
