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
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public MarkAllAsReadCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Tüm NEW durumundaki mesajları bul
                var newMessages = await _context.ContactMessages
                    .Where(m => m.Status == ContactMessageStatus.New)
                    .ToListAsync(cancellationToken);

                if (newMessages.Any())
                {
                    foreach (var message in newMessages)
                    {
                        message.Status = ContactMessageStatus.Read;
                        message.UpdatedDate = DateTime.UtcNow;
                        message.UpdatedBy = _currentUser.UserEmail;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Mesajlar okundu olarak işaretlenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
