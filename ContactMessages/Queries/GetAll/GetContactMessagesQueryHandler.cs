using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.ContactMessages.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetAll
{
    public class GetContactMessagesQueryHandler : IRequestHandler<GetContactMessagesQuery, PagedResult<ContactMessageListDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetContactMessagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ContactMessageListDto>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ContactMessages.AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x =>
                    x.FirstName.Contains(request.SearchTerm) ||
                    x.LastName.Contains(request.SearchTerm) ||
                    x.Email.Contains(request.SearchTerm) ||
                    x.Subject.Contains(request.SearchTerm) ||
                    x.Message.Contains(request.SearchTerm) ||
                    (x.Company != null && x.Company.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= request.EndDate.Value.AddDays(1));
            }

            // Sorting
            query = request.SortBy?.ToLower() switch
            {
                "firstname" => request.SortDescending ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName),
                "lastname" => request.SortDescending ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName),
                "email" => request.SortDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
                "subject" => request.SortDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject),
                "status" => request.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                _ => request.SortDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate)
            };

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
                    HasReply = !string.IsNullOrEmpty(x.AdminReply),
                    CreatedDate = x.CreatedDate,
                    RepliedAt = x.RepliedAt
                })
                .ToListAsync(cancellationToken);

            return PagedResult<ContactMessageListDto>.Create(contactMessages, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
