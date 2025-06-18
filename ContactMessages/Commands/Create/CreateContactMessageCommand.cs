using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Create
{
    public record CreateContactMessageCommand : IRequest<Result<int>>
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? Company { get; init; }
        public string Subject { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }
}
