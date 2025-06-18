using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.DTOs
{
    public class ContactMessageReplyDto
    {
        public int Id { get; set; }
        public string AdminReply { get; set; } = string.Empty;
    }
}
