using Application.Common.Models;
using Application.Features.ContactMessages.DTOs;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetAll
{
    public record GetContactMessagesQuery : IRequest<PagedResult<ContactMessageListDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public ContactMessageStatus? Status { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string? SortBy { get; init; } = "CreatedDate";
        public bool SortDescending { get; init; } = true;
    }
}
