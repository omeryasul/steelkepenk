using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Create
{
    public class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateContactMessageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new ContactMessage
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Company = request.Company,
                    Subject = request.Subject,
                    Message = request.Message,
                    Status = ContactMessageStatus.New,
                    IpAddress = request.IpAddress,
                    UserAgent = request.UserAgent,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ContactMessages.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"İletişim mesajı oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
