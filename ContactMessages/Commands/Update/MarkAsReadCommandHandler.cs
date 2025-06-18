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

namespace Application.Features.ContactMessages.Commands.Update
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public MarkAsReadCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.ContactMessages
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("İletişim mesajı bulunamadı.");

                if (entity.Status == ContactMessageStatus.New)
                {
                    entity.Status = ContactMessageStatus.Read;
                    entity.UpdatedDate = DateTime.UtcNow;
                    entity.UpdatedBy = _currentUser.UserEmail;

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Mesaj okundu olarak işaretlenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
