using CyberSecurityChatbot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace CyberSecurityChatBotWPF
{
    public partial class MainWindow : Window
    {
        // Activity log entry struct
        private struct ActivityLogEntry
        {
            public DateTime Timestamp;
            public string Description;
            public string Category; // e.g. "Task", "Reminder", "Quiz", "NLP", "Topic"
        }

        // Chatbot memory and tools
        private static UserProfile user = new UserProfile();
        private static Random random = new Random();
        private static string lastUserTopic = "";
        private static Dictionary<string, int> interestTipIndex = new();
        private static HashSet<string> givenTipInterests = new();

        private Topic currentTopic = Topic.None;

        // Windows
        private TaskWindow taskWindow;
        private CyberTask lastAddedTask = null;

        // Activity log related fields
        private List<ActivityLogEntry> activityLog = new();
        private int logDisplayCount = 5; // Show 5 entries by default
        private string lastDetectedTopicOrKeyword = "None";

        public MainWindow()
        {
            InitializeComponent();
            BotSay("Hello! 👋 Welcome to the Cybersecurity ChatBot.");
            
        }

        // Append user message to chat history display
        private void AppendUser(string message)
        {
            ChatHistoryTextBlock.Text += $"You: {message}\n";
        }

        // Bot message output helper
        private void BotSay(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }

        // Add an entry to the activity log
        private void AddToActivityLog(string description, string category = "General")
        {
            activityLog.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Description = description,
                Category = category
            });

            if (activityLog.Count > 100)
                activityLog.RemoveAt(0); // keep max 100 entries
        }

        // Update and log last conversation topic or keyword
        private void UpdateLastTopicOrKeyword(string topic)
        {
            lastDetectedTopicOrKeyword = topic;
            AddToActivityLog($"Detected conversation topic/keyword: {topic}", "Topic");
        }

        // Display activity log entries with pagination
        private void ShowActivityLog(int countToShow)
        {
            if (activityLog.Count == 0)
            {
                BotSay("Activity log is empty.");
                return;
            }

            int startIndex = Math.Max(activityLog.Count - countToShow, 0);
            var entriesToShow = activityLog.Skip(startIndex).Take(countToShow).ToList();

            string logMessage = $"Showing last {entriesToShow.Count} actions (out of {activityLog.Count}):\n" +
                string.Join("\n", entriesToShow.Select((entry, i) =>
                    $"{startIndex + i + 1}. [{entry.Timestamp:G}] [{entry.Category}] {entry.Description}"));

            BotSay(logMessage);

            if (activityLog.Count > countToShow)
            {
                BotSay("Type 'show more log' to see more entries.");
            }
        }

        // User intents
        private enum UserIntent
        {
            AddTask,
            SetReminder,
            StartQuiz,
            ShowActivityLog,
            Unknown
        }

        // Detect user intent with extended keyword detection, including activity log commands
        private (UserIntent intent, string extractedText, int? days) DetectUserIntent(string input)
        {
            input = input.ToLower().Trim();

            // Show more log pagination
            if (input.Contains("show more log") || input.Contains("more activity log"))
                return (UserIntent.ShowActivityLog, "more", null);

            // Show activity log commands
            if (input.Contains("show activity log") || input.Contains("activity log") || input.Contains("show log") || input.Contains("what have you done"))
                return (UserIntent.ShowActivityLog, "", null);

            // Add Task
            if (input.Contains("add task") || input.Contains("create task") || input.Contains("note to") || input.Contains("add a task to"))
            {
                var match = Regex.Match(input, @"(?:add|create|note)\s+(?:a\s+)?task\s+(?:to\s+)?(.+)");
                return (UserIntent.AddTask, match.Success ? match.Groups[1].Value.Trim() : "", null);
            }

            // Set Reminder - Improved regex and logic to extract task and days more reliably
            if (input.Contains("remind me") || input.Contains("set a reminder"))
            {
                // Pattern to extract "remind me to <task> (tomorrow|today|in X days)"
                var pattern = @"remind me(?: to)? (?<task>.+?)(?: (tomorrow|today|in (\d+) days))?$";
                var match = Regex.Match(input, pattern);

                if (match.Success)
                {
                    string taskText = match.Groups["task"].Value.Trim();
                    int? days = null;

                    if (input.EndsWith("tomorrow"))
                        days = 1;
                    else if (input.EndsWith("today"))
                        days = 0;
                    else if (int.TryParse(match.Groups[3].Value, out int parsedDays))
                        days = parsedDays;

                    return (UserIntent.SetReminder, taskText, days);
                }
            }

            // Quiz Start
            if (input.Contains("start quiz") || input.Contains("quiz me") || input.Contains("take quiz"))
                return (UserIntent.StartQuiz, "", null);

            return (UserIntent.Unknown, "", null);
        }

        // Main chat message handler
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputTextBox.Text.Trim();
            UserInputTextBox.Clear();

            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUser(input);

            var (intent, text, days) = DetectUserIntent(input);

            switch (intent)
            {
                case UserIntent.AddTask:
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        BotSay("Sure! What would you like the task to be?");
                        return;
                    }

                    if (taskWindow == null) taskWindow = new TaskWindow();
                    taskWindow.Show();

                    lastAddedTask = new CyberTask { Title = text };
                    taskWindow.Tasks.Add(lastAddedTask);
                    taskWindow.RefreshList();

                    BotSay($"Task added: \"{text}\". Would you like to set a reminder for this task?");
                    AddToActivityLog($"Task added: \"{text}\"", "Task");
                    UpdateLastTopicOrKeyword("Task");
                    break;

                case UserIntent.SetReminder:
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        BotSay("What should I remind you about?");
                        return;
                    }

                    var task = new CyberTask { Title = text };

                    if (days.HasValue)
                    {
                        task.ReminderDate = DateTime.Today.AddDays(days.Value);
                        BotSay($"Reminder set for \"{text}\" in {days.Value} days.");
                        AddToActivityLog($"Reminder set for \"{text}\" in {days.Value} days", "Reminder");
                    }
                    else
                    {
                        BotSay($"Task added: \"{text}\". You can open the task manager to set a specific reminder.");
                        AddToActivityLog($"Reminder added for \"{text}\" (no specific date)", "Reminder");
                    }

                    if (taskWindow == null) taskWindow = new TaskWindow();
                    taskWindow.Tasks.Add(task);
                    taskWindow.RefreshList();

                    UpdateLastTopicOrKeyword("Reminder");
                    break;

                case UserIntent.StartQuiz:
                    AddToActivityLog("Quiz started", "Quiz");
                    UpdateLastTopicOrKeyword("Quiz");
                    OpenQuiz_Click(sender, e);
                    break;

                case UserIntent.ShowActivityLog:
                    if (text == "more")
                    {
                        logDisplayCount += 5; // Increase displayed logs by 5 on each 'show more' command
                        ShowActivityLog(logDisplayCount);
                    }
                    else
                    {
                        logDisplayCount = 5; // Reset to default
                        ShowActivityLog(logDisplayCount);
                    }
                    break;

                default:
                    BotSay("Sorry, I don't understand that yet. Try asking about tasks, reminders, or quizzes.");
                    break;
            }
        }

        // Open Task Manager window
        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            if (taskWindow == null)
            {
                taskWindow = new TaskWindow();
                taskWindow.Closed += (s, args) => taskWindow = null;
            }

            taskWindow.Show();
            taskWindow.Activate();
        }

        // Open Quiz Window
        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
        }

        // Tip generator (unchanged)
        private static string GetRotatingTip(string input)
        {
            input = input.ToLower();

            foreach (string interest in user.Interests)
            {
                if (givenTipInterests.Contains(interest) || interest != lastUserTopic.ToLower())
                    continue;

                if (!interestTipIndex.ContainsKey(interest))
                    interestTipIndex[interest] = 0;

                if (!Enum.TryParse<Topic>(interest, true, out Topic topic)) continue;
                if (!TipLibrary.TipsByTopic.ContainsKey(topic)) continue;

                var tips = TipLibrary.TipsByTopic[topic];
                string tip = tips[interestTipIndex[interest]];

                interestTipIndex[interest] = (interestTipIndex[interest] + 1) % tips.Length;
                givenTipInterests.Add(interest);

                return $"💡 As someone interested in {interest}, here's a tip: {tip}";
            }

            return null;
        }

        // User info parser (unchanged)
        private static void ParseUserInfo(string input)
        {
            input = input.ToLower();
            bool updated = false;

            var ageMatch = Regex.Match(input, @"(\d{1,3})\s*(year|yr)s?\s*old");
            if (ageMatch.Success && int.TryParse(ageMatch.Groups[1].Value, out int age))
            {
                if (!user.Age.HasValue)
                {
                    user.Age = age;
                    updated = true;
                }
            }

            if (input.Contains("student")) { user.Role = "student"; updated = true; }
            else if (input.Contains("engineer")) { user.Role = "engineer"; updated = true; }
            else if (input.Contains("teacher") || input.Contains("professor")) { user.Role = "teacher"; updated = true; }

            var interestMatch = Regex.Match(input, @"(?:interested in|learning about)\s+([a-z\s,]+)");
            if (interestMatch.Success)
            {
                string[] interests = interestMatch.Groups[1].Value.Split(new[] { ",", " and " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var interest in interests)
                {
                    string clean = interest.Trim();
                    if (!user.Interests.Contains(clean))
                    {
                        user.AddInterest(clean);
                        updated = true;
                    }
                }
            }

            if (updated)
            {
                string summary = $"Got it! Age: {user.Age ?? 0}, Role: {user.Role}, Interests: {string.Join(", ", user.Interests)}.";
                MessageBox.Show(summary, "User Info Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}