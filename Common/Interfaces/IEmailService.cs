using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendContactReplyAsync(string to, string customerName, string originalMessage, string replyMessage);
        Task SendWelcomeEmailAsync(string to, string userName);
        Task SendPasswordResetEmailAsync(string to, string resetToken);
    }
}
