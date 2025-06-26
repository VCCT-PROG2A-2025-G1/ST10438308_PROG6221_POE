using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CyberSecurityChatBotWPF
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public List<CyberTask> Tasks { get; set; } = new();

        public TaskWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        public void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new CyberTask
            {
                Title = TitleBox.Text,
                Description = DescriptionBox.Text,
                ReminderDate = ReminderPicker.SelectedDate,
                IsCompleted = false
            };

            if (string.IsNullOrWhiteSpace(task.Title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            Tasks.Add(task);
            RefreshList();

            TitleBox.Clear();
            DescriptionBox.Clear();
            ReminderPicker.SelectedDate = null;
        }

        public void RefreshList()
        {
            TaskList.Items.Clear();
            foreach (var task in Tasks)
            {
                TaskList.Items.Add(task.ToString());
            }
        }

        public void MarkComplete_Click(object sender, RoutedEventArgs e)
        {
            int index = TaskList.SelectedIndex;
            if (index >= 0 && index < Tasks.Count)
            {
                Tasks[index].IsCompleted = true;
                RefreshList();
            }
            else
            {
                MessageBox.Show("Please select a task to mark as complete.");
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            int index = TaskList.SelectedIndex;
            if (index >= 0 && index < Tasks.Count)
            {
                Tasks.RemoveAt(index);
                RefreshList();
            }
            else
            {
                MessageBox.Show("Please select a task to delete.");
            }
        }

        public void CheckReminders_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            List<string> dueTasks = new();

            foreach (var task in Tasks)
            {
                if (task.ReminderDate.HasValue && task.ReminderDate.Value.Date == today && !task.IsCompleted)
                {
                    dueTasks.Add($"🔔 Reminder: {task.Title} is due today!");
                }
            }

            if (dueTasks.Count > 0)
            {
                MessageBox.Show(string.Join("\n", dueTasks), "Today's Reminders");
            }
            else
            {
                MessageBox.Show("No reminders for today!", "Reminders");
            }
        }
    }
}