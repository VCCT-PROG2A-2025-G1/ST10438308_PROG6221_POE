using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace CyberSecurityChatBotWPF
{
    public partial class MainWindow : Window
    {
        private TaskWindow taskWindow;
        private CyberTask lastAddedTask;
        private List<string> activityLog = new();
        private int activityPage = 0;
        private const int LogPageSize = 5;



        public MainWindow()
        {
            InitializeComponent();
            BotSay("Welcome to the Cybersecurity ChatBot!");

            // Subscribe to quiz event
            QuizWindow.QuizCompleted += OnQuizCompleted;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputTextBox.Text.Trim();
            UserInputTextBox.Clear();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUser(input);

            var (intent, extractedText, days) = DetectUserIntent(input);

            switch (intent)
            {
                case UserIntent.AddTask:
                    HandleAddTask(extractedText);
                    break;

                case UserIntent.SetReminder:
                    HandleSetReminder(extractedText, days);
                    break;

                case UserIntent.StartQuiz:
                    OpenQuiz_Click();
                    LogActivity("[Quiz] Quiz started");
                    break;

                case UserIntent.ShowLog:
                    activityPage = 0;
                    ShowActivityLog();
                    break;

                case UserIntent.ShowMoreLog:
                    activityPage++;
                    ShowActivityLog();
                    break;

                case UserIntent.OpenTaskManager:
                    OpenTaskManager();
                    break;

                default:
                    BotSay("Sorry, I don't understand that yet. Try asking about tasks, reminders, or quizzes.");
                    break;
            }
        }
        
        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            EnsureTaskWindow();
            taskWindow.Show();
            BotSay("Task Manager opened.");
        }

        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
            BotSay("Cybersecurity quiz started.");
        }

        private void HandleAddTask(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                BotSay("What should I name the task?");
                return;
            }

            lastAddedTask = new CyberTask { Title = text };
            EnsureTaskWindow();
            taskWindow.Tasks.Add(lastAddedTask);
            taskWindow.RefreshList();

            BotSay($"Task added: \"{text}\". Would you like to set a reminder for this task?");
            LogActivity($"[Task] Task added: \"{text}\"");
            LogActivity("[Topic] Detected conversation topic: Task");
        }

        private void OnQuizCompleted(object? sender, QuizCompletedEventArgs e)
        {
            string message = $"You scored {e.Score}/{e.TotalQuestions} ({Math.Round(e.Percentage)}%).";
            BotSay(message);
            LogActivity($"[Quiz] Completed quiz: {e.Score}/{e.TotalQuestions} correct ({Math.Round(e.Percentage)}%)");

            // Optional: Log answered questions
            foreach (var q in e.QuestionsAnswered)
            {
                LogActivity($"[Quiz] Answered: \"{q}\"");
            }
        }
        private void HandleSetReminder(string text, int? days)
        {
            text = text?.Trim();

            // Update last task reminder if no new task text provided
            if (days.HasValue && string.IsNullOrWhiteSpace(text) && lastAddedTask != null)
            {
                lastAddedTask.ReminderDate = DateTime.Today.AddDays(days.Value);
                taskWindow?.RefreshList();

                BotSay($"Reminder set for \"{lastAddedTask.Title}\" in {days.Value} days.");
                LogActivity($"[Reminder] Reminder set for \"{lastAddedTask.Title}\" in {days.Value} days");
                LogActivity("[Topic] Detected conversation topic: Reminder");
                return;
            }

            // No text provided to create a new reminder
            if (string.IsNullOrWhiteSpace(text))
            {
                BotSay("What should I remind you about?");
                return;
            }

            // Create new task with optional reminder
            var newTask = new CyberTask
            {
                Title = text,
                ReminderDate = days.HasValue ? DateTime.Today.AddDays(days.Value) : null
            };

            EnsureTaskWindow();
            taskWindow.Tasks.Add(newTask);
            taskWindow.RefreshList();
            lastAddedTask = newTask;

            if (days.HasValue)
            {
                BotSay($"Reminder set for \"{text}\" in {days.Value} days.");
            }
            else
            {
                BotSay($"Task added: \"{text}\". You can set a reminder later.");
            }

            LogActivity($"[Reminder] Reminder added for \"{text}\"" + (days.HasValue ? $" in {days.Value} days" : ""));
            LogActivity("[Topic] Detected conversation topic: Reminder");
        }

        private void ShowActivityLog()
        {
            var slice = activityLog.Skip(activityPage * LogPageSize).Take(LogPageSize).ToList();

            if (slice.Count == 0)
            {
                BotSay("No more activity log entries.");
                return;
            }

            string message = $"Showing {slice.Count} of {activityLog.Count} actions:\n" +
                             string.Join("\n", slice.Select((entry, i) => $"{i + 1 + activityPage * LogPageSize}. {entry}"));

            BotSay(message);

            if ((activityPage + 1) * LogPageSize < activityLog.Count)
                BotSay("Type 'show more log' to view additional entries.");
        }

        private void LogActivity(string entry)
        {
            activityLog.Add($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {entry}");
        }

        private void BotSay(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }

        private void AppendUser(string input)
        {
            ChatHistoryTextBlock.Text += $"You: {input}\n";
        }

        private void EnsureTaskWindow()
        {
            if (taskWindow == null || !taskWindow.IsVisible)
                taskWindow = new TaskWindow();
        }

        private void OpenTaskManager()
        {
            EnsureTaskWindow();
            taskWindow.Show();
            BotSay("Task manager opened.");
        }

        private void OpenQuiz_Click()
        {
            new QuizWindow().Show();
        }

        private enum UserIntent
        {
            AddTask,
            SetReminder,
            StartQuiz,
            ShowLog,
            ShowMoreLog,
            OpenTaskManager,
            Unknown
        }

        private (UserIntent, string, int?) DetectUserIntent(string input)
        {
            input = input.ToLower();

            if (input.Contains("task manager")) return (UserIntent.OpenTaskManager, "", null);
            if (input.Contains("start quiz") || input.Contains("quiz me")) return (UserIntent.StartQuiz, "", null);
            if (input.Contains("show activity") || input.Contains("what have you done")) return (UserIntent.ShowLog, "", null);
            if (input.Contains("show more log")) return (UserIntent.ShowMoreLog, "", null);

            var addMatch = Regex.Match(input, @"(?:add|create)(?: a)? task(?: to)? (.+)");
            if (addMatch.Success) return (UserIntent.AddTask, addMatch.Groups[1].Value, null);

            if (input.Contains("remind me") || input.Contains("set a reminder"))
            {
                var match = Regex.Match(input, @"remind me to (.+?) in (\d+) days");
                if (match.Success)
                {
                    string taskText = match.Groups[1].Value.Trim();
                    int days = int.Parse(match.Groups[2].Value);
                    return (UserIntent.SetReminder, taskText, days);
                }

                // fallback: try to get task only
                var fallback = Regex.Match(input, @"remind me to (.+)");
                if (fallback.Success)
                {
                    return (UserIntent.SetReminder, fallback.Groups[1].Value.Trim(), null);
                }

                return (UserIntent.SetReminder, "", null);
            }

            var yesRemind = Regex.Match(input, @"yes.*remind.*in (\d+) days");
            if (yesRemind.Success)
            {
                int d = int.Parse(yesRemind.Groups[1].Value);
                return (UserIntent.SetReminder, "", d);
            }

            return (UserIntent.Unknown, "", null);
        }
    }
}
