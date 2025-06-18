using Application.Common.Models;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Update
{
    public record UpdateContactMessageCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? Company { get; init; }
        public string Subject { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public ContactMessageStatus Status { get; init; }
        public string? AdminReply { get; init; }
    }
}
