// ChatBotUI.cs (WPF version)
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberSecurityChatbot
{
    public static class ChatBotUI
    {
        // This method adds messages to a WPF ListBox or other ItemsControl to simulate chat
        public static async Task PrintTypingAsync(ItemsControl chatPanel, string text, Brush color, int delayMs = 20)
        {
            foreach (char c in text)
            {
                
                await Task.Delay(delayMs);
            }

            chatPanel.Items.Add(new TextBlock
            {
                Text = text,
                Foreground = color,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Margin = new System.Windows.Thickness(5)
            });
        }
    }
}
//-------------------------------END OF FILE----------------------//
