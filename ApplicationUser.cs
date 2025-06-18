using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Persistence.Context;

namespace Persistence
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Domain User entity'ye mapping için
        public User ToUser()
        {
            return new User
            {
                UserName = UserName ?? string.Empty,
                Email = Email ?? string.Empty,
                FirstName = FirstName,
                LastName = LastName,
                Phone = PhoneNumber,
                Avatar = Avatar,
                IsAdmin = IsAdmin,
                IsActive = IsActive,
                EmailConfirmed = EmailConfirmed,
                LastLoginAt = LastLoginAt,
                CreatedDate = CreatedDate,
                UpdatedDate = UpdatedDate,
                CreatedBy = CreatedBy,
                UpdatedBy = UpdatedBy
            };
        }
    }
}
