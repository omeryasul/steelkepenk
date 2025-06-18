using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.ContactMessages.DTOs;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetUnread
{
    public class GetUnreadContactMessagesQueryHandler : IRequestHandler<GetUnreadContactMessagesQuery, PagedResult<ContactMessageListDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUnreadContactMessagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ContactMessageListDto>> Handle(GetUnreadContactMessagesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ContactMessages
                .Where(x => x.Status == ContactMessageStatus.New)
                .OrderByDescending(x => x.CreatedDate);

            var totalCount = await query.CountAsync(cancellationToken);

            var contactMessages = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ContactMessageListDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Company = x.Company,
                    Subject = x.Subject,
                    Status = x.Status,
                    StatusText = x.Status.ToString(),
                    HasReply = false,
                    CreatedDate = x.CreatedDate,
                    RepliedAt = null
                })
                .ToListAsync(cancellationToken);

            return PagedResult<ContactMessageListDto>.Create(contactMessages, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
