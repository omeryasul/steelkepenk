using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Delete
{
    public class DeleteContactMessageCommandHandler : IRequestHandler<DeleteContactMessageCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteContactMessageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.ContactMessages
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("İletişim mesajı bulunamadı.");

                _context.ContactMessages.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İletişim mesajı silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}
