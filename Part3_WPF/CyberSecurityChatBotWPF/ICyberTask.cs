using System.ComponentModel;

namespace CyberSecurityChatBotWPF
{
    public interface ICyberTask
    {
        DateTime? CompletedDate { get; set; }
        DateTime CreatedDate { get; set; }
        string Description { get; set; }
        bool IsCompleted { get; set; }
       
        DateTime? ReminderDate { get; set; }
      
        string Title { get; set; }
        
        event PropertyChangedEventHandler PropertyChanged;

        string ToString();
    }
}