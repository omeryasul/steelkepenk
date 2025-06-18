using Application.Common.Interfaces;
using Application.Features.ContactMessages.DTOs;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetStats
{
    public class GetContactMessageStatsQueryHandler : IRequestHandler<GetContactMessageStatsQuery, ContactMessageStatsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetContactMessageStatsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContactMessageStatsDto> Handle(GetContactMessageStatsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var thisWeek = today.AddDays(-(int)today.DayOfWeek);
            var thisMonth = new DateTime(now.Year, now.Month, 1);

            var stats = new ContactMessageStatsDto
            {
                TotalMessages = await _context.ContactMessages.CountAsync(cancellationToken),
                NewMessages = await _context.ContactMessages.CountAsync(x => x.Status == ContactMessageStatus.New, cancellationToken),
                ReadMessages = await _context.ContactMessages.CountAsync(x => x.Status == ContactMessageStatus.Read, cancellationToken),
                RepliedMessages = await _context.ContactMessages.CountAsync(x => x.Status == ContactMessageStatus.Replied, cancellationToken),
                ClosedMessages = await _context.ContactMessages.CountAsync(x => x.Status == ContactMessageStatus.Closed, cancellationToken),
                TodayMessages = await _context.ContactMessages.CountAsync(x => x.CreatedDate >= today, cancellationToken),
                ThisWeekMessages = await _context.ContactMessages.CountAsync(x => x.CreatedDate >= thisWeek, cancellationToken),
                ThisMonthMessages = await _context.ContactMessages.CountAsync(x => x.CreatedDate >= thisMonth, cancellationToken)
            };

            return stats;
        }
    }
}
