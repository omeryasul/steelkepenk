using Domain.Enums;
using System;

namespace Application.Features.ContactMessages.DTOs
{
    public class ContactMessageDetailDto
    {
        public int Id { get; set; }

        // İletişim Bilgileri
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }

        // Mesaj Bilgileri
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ContactMessageStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;

        // Admin Yanıtı
        public string? AdminReply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string? RepliedBy { get; set; }

        // Teknik Bilgiler
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }}
    }
