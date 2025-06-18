using Application.Common.Models;
using Application.Features.ContactMessages.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetUnread
{
    public record GetUnreadContactMessagesQuery : IRequest<PagedResult<ContactMessageListDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
