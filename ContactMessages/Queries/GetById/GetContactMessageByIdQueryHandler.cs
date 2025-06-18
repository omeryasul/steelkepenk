using Application.Common.Interfaces;
using Application.Features.ContactMessages.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetById
{
    public class GetContactMessageByIdQueryHandler : IRequestHandler<GetContactMessageByIdQuery, ContactMessageDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetContactMessageByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContactMessageDetailDto?> Handle(GetContactMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var contactMessage = await _context.ContactMessages
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (contactMessage == null)
                return null;

            return new ContactMessageDetailDto
            {
                Id = contactMessage.Id,
                FirstName = contactMessage.FirstName,
                LastName = contactMessage.LastName,
                FullName = contactMessage.FullName,
                Email = contactMessage.Email,
                Phone = contactMessage.Phone,
                Company = contactMessage.Company,
                Subject = contactMessage.Subject,
                Message = contactMessage.Message,
                Status = contactMessage.Status,
                StatusText = contactMessage.Status.ToString(),
                AdminReply = contactMessage.AdminReply,
                RepliedAt = contactMessage.RepliedAt,
                RepliedBy = contactMessage.RepliedBy,
                IpAddress = contactMessage.IpAddress,
                UserAgent = contactMessage.UserAgent,
                CreatedDate = contactMessage.CreatedDate,
                UpdatedDate = contactMessage.UpdatedDate,
                CreatedBy = contactMessage.CreatedBy,
                UpdatedBy = contactMessage.UpdatedBy
            };
        }
    }
}
