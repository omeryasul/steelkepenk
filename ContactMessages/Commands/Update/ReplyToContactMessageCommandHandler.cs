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
    public class ReplyToContactMessageCommandHandler : IRequestHandler<ReplyToContactMessageCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ReplyToContactMessageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(ReplyToContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.ContactMessages
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("İletişim mesajı bulunamadı.");

                entity.AdminReply = request.AdminReply;
                entity.RepliedAt = DateTime.UtcNow;
                entity.RepliedBy = _currentUser.UserEmail;
                entity.Status = ContactMessageStatus.Replied;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                await _context.SaveChangesAsync(cancellationToken);

                // E-posta gönderimi için ayrı bir servis kullanabilirsiniz
                // await _emailService.SendReplyEmailAsync(entity.Email, entity.FullName, entity.Subject, request.AdminReply);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Mesaj yanıtlanırken hata oluştu: {ex.Message}");
            }
        }
    }
}
