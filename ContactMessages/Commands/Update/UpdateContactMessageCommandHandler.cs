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
    public class UpdateContactMessageCommandHandler : IRequestHandler<UpdateContactMessageCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateContactMessageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.ContactMessages
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("İletişim mesajı bulunamadı.");

                entity.FirstName = request.FirstName;
                entity.LastName = request.LastName;
                entity.Email = request.Email;
                entity.Phone = request.Phone;
                entity.Company = request.Company;
                entity.Subject = request.Subject;
                entity.Message = request.Message;
                entity.Status = request.Status;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                // Admin yanıtı eklenmişse
                if (!string.IsNullOrEmpty(request.AdminReply) && string.IsNullOrEmpty(entity.AdminReply))
                {
                    entity.AdminReply = request.AdminReply;
                    entity.RepliedAt = DateTime.UtcNow;
                    entity.RepliedBy = _currentUser.UserEmail;
                    entity.Status = ContactMessageStatus.Replied;
                }
                else if (!string.IsNullOrEmpty(request.AdminReply))
                {
                    entity.AdminReply = request.AdminReply;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İletişim mesajı güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
