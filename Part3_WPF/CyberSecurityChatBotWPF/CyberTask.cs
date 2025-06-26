using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatBotWPF
{
    public class CyberTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        public override string ToString()
        {
            string status = IsCompleted ? "✅ Completed" : "⏳ Pending";
            string reminder = ReminderDate.HasValue ? $"⏰ Reminder: {ReminderDate.Value.ToShortDateString()}" : "No reminder set";
            return $"{Title} - {Description}\n{status} | {reminder}";
        }
    }
}
