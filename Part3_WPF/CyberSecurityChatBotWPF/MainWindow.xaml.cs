using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CyberSecurityChatBotWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<CyberTask> userTasks = new List<CyberTask>();
        private BotService bot = new BotService(); // uses your chatbot logic

        public MainWindow()
        {
            InitializeComponent();
        }

        // 🔘 Chatbot message processing
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            string response = bot.ProcessInput(input);

            // 🚨 Check if the response is a task signal
            if (response.StartsWith("##ADD_TASK##"))
            {
                string[] parts = response.Replace("##ADD_TASK##", "").Split('|');
                string title = parts[0].Trim();
                string description = parts.Length > 1 ? parts[1].Trim() : "Cybersecurity task.";

                var task = new CyberTask
                {
                    Title = title,
                    Description = description
                };

                userTasks.Add(task);
                TaskList.Items.Add(task);

                ChatOutput.Text += $"\nYou: {input}\nCyberBot: Task \"{title}\" added with description \"{description}\". Would you like to set a reminder?\n";
            }
            else
            {
                ChatOutput.Text += $"\nYou: {input}\nCyberBot: {response}\n";
            }

            UserInput.Clear();
        }

        // ✅ Add Task
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new CyberTask
            {
                Title = TaskTitleBox.Text,
                Description = TaskDescriptionBox.Text,
                ReminderDate = TaskReminderPicker.SelectedDate
            };

            if (string.IsNullOrWhiteSpace(task.Title) || string.IsNullOrWhiteSpace(task.Description))
            {
                MessageBox.Show("Please provide both title and description.");
                return;
            }

            userTasks.Add(task);
            TaskList.Items.Add(task);

            ChatOutput.Text += $"\nCyberBot: Task \"{task.Title}\" added. " +
                               (task.ReminderDate.HasValue ? $"I'll remind you on {task.ReminderDate.Value:MMM dd, yyyy}." : "") + "\n";

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskReminderPicker.SelectedDate = null;
        }

        // ❌ Delete Task
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is CyberTask selectedTask)
            {
                userTasks.Remove(selectedTask);
                TaskList.Items.Remove(selectedTask);
                ChatOutput.Text += $"\nCyberBot: Task \"{selectedTask.Title}\" has been deleted.\n";
            }
            else
            {
                MessageBox.Show("Please select a task to delete.");
            }
        }

        // ✔ Mark as Completed
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.IsCompleted = true;
                TaskList.Items.Refresh(); // Refresh ListBox display
                ChatOutput.Text += $"\nCyberBot: Task \"{selectedTask.Title}\" marked as completed. ✅\n";
            }
            else
            {
                MessageBox.Show("Please select a task to mark as completed.");
            }
        }
    }
}