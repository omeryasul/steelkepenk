using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserEmail { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}
