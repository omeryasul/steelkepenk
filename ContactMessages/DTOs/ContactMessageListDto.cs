using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.DTOs
{
    public class ContactMessageListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string Subject { get; set; } = string.Empty;
        public ContactMessageStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public bool HasReply { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RepliedAt { get; set; }
    }
}
