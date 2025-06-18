using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.DTOs
{
    public class ContactMessageStatsDto
    {
        public int TotalMessages { get; set; }
        public int NewMessages { get; set; }
        public int ReadMessages { get; set; }
        public int RepliedMessages { get; set; }
        public int ClosedMessages { get; set; }
        public int TodayMessages { get; set; }
        public int ThisWeekMessages { get; set; }
        public int ThisMonthMessages { get; set; }
    }
}
