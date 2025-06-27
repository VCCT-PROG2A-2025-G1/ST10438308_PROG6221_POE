using System;

namespace CyberSecurityChatBotWPF
{
    //class representing a cyber security task
    public class CyberTask
    {
        public string Title { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            var reminderText = ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value.ToShortDateString()})" : "";
            var status = IsCompleted ? " [COMPLETED]" : "";
            return $"{Title}{reminderText}{status}";
        }
    }
}
//-----------------------------END OF FILE-----------------------------//
