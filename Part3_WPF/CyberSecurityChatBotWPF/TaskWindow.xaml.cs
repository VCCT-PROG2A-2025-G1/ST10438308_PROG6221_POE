using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CyberSecurityChatBotWPF
{
    public partial class TaskWindow : Window
    {
        // List to hold tasks
        public List<CyberTask> Tasks { get; set; } = new();

        //--------------------CONSTRUCTOR--------------------//
        public TaskWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        //--------------------EVENT HANDLERS--------------------//
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new CyberTask
            {
                Title = TitleBox.Text,
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
            ReminderPicker.SelectedDate = null;
        }

        //--------------------METHODS--------------------//
        // Refresh the task list display
        public void RefreshList()
        {
            TaskList.Items.Clear();
            foreach (var task in Tasks)
            {
                TaskList.Items.Add(task);
            }
        }

        // Mark the selected task as complete
        private void MarkComplete_Click(object sender, RoutedEventArgs e)
        {
            int index = TaskList.SelectedIndex;
            if (index >= 0 && index < Tasks.Count)
            {
                Tasks[index].IsCompleted = true;
                RefreshList();
            }
        }

        // Delete the selected task from the list
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            int index = TaskList.SelectedIndex;
            if (index >= 0 && index < Tasks.Count)
            {
                Tasks.RemoveAt(index);
                RefreshList();
            }
        }

        // Check for reminders for today's tasks
        private void CheckReminders_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            List<string> reminders = new();

            foreach (var task in Tasks)
            {
                if (task.ReminderDate.HasValue && task.ReminderDate.Value.Date == today && !task.IsCompleted)
                {
                    reminders.Add($"🔔 Reminder: {task.Title} is due today!");
                }
            }

            string message = reminders.Count > 0
                ? string.Join("\n", reminders)
                : "No reminders for today.";

            MessageBox.Show(message, "Today's Reminders");
        }
    }
}
//------------------------------------------------------------END OF FILE-----------------------------------------------------//
