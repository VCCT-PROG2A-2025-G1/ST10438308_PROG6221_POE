using CyberSecurityChatbot;
using System.Text.RegularExpressions;
using System.Windows;

namespace CyberSecurityChatBotWPF
{
    public partial class MainWindow : Window
    {
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

        private void AppendUser(string message)
        {
            ChatHistoryTextBlock.Text += $"You: {message}\n";
        }

        public MainWindow()
        {
            InitializeComponent();
            BotSay("Hello! 👋 Welcome to the Cybersecurity ChatBot.");
            BotSay("What is your name?");
        }

        private enum UserIntent
        {
            AddTask,
            SetReminder,
            StartQuiz,
            ShowHistory,
            Unknown
        }

        private (UserIntent intent, string extractedText, int? days) DetectUserIntent(string input)
        {
            input = input.ToLower();

            // Add Task
            if (input.Contains("add task") || input.Contains("create task") || input.Contains("note to") || input.Contains("add a task to"))
            {
                var match = Regex.Match(input, @"(?:add|create|note)\s+(?:a\s+)?task\s+(?:to\s+)?(.+)");
                return (UserIntent.AddTask, match.Success ? match.Groups[1].Value : "", null);
            }

            // Set Reminder
            if (input.Contains("remind me") || input.Contains("set a reminder"))
            {
                var match = Regex.Match(input, @"(?:remind me|set a reminder)\s+(?:to\s+)?(.+?)\s*(?:in\s+(\d+)\s+days)?");
                if (match.Success)
                {
                    string taskText = match.Groups[1].Value.Trim();
                    int? days = null;
                    if (int.TryParse(match.Groups[2].Value, out int d))
                        days = d;

                    return (UserIntent.SetReminder, taskText, days);
                }
            }

            // Quiz Start
            if (input.Contains("start quiz") || input.Contains("quiz me") || input.Contains("take quiz"))
            {
                return (UserIntent.StartQuiz, "", null);
            }

            // Summary / History
            if (input.Contains("what have you done") || input.Contains("show my actions") || input.Contains("summary"))
            {
                return (UserIntent.ShowHistory, "", null);
            }

            return (UserIntent.Unknown, "", null);
        }

        // Main chat handler
        private List<string> actionHistory = new();

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
                    actionHistory.Add($"Task added: \"{text}\" (no reminder set)");
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
                        actionHistory.Add($"Reminder set for \"{text}\" in {days.Value} days");
                    }
                    else
                    {
                        BotSay($"Task added: \"{text}\". You can open the task manager to set a specific reminder.");
                        actionHistory.Add($"Task added: \"{text}\" (no reminder set)");
                    }

                    if (taskWindow == null) taskWindow = new TaskWindow();
                    taskWindow.Tasks.Add(task);
                    taskWindow.RefreshList();
                    break;

                case UserIntent.StartQuiz:
                    OpenQuiz_Click(sender, e);
                    actionHistory.Add("Quiz started");
                    break;

                case UserIntent.ShowHistory:
                    if (actionHistory.Count == 0)
                    {
                        BotSay("I haven’t done anything for you yet.");
                    }
                    else
                    {
                        BotSay("Here’s a summary of recent actions:\n" +
                            string.Join("\n", actionHistory.Select((a, i) => $"{i + 1}. {a}")));
                    }
                    break;

                default:
                    BotSay("Sorry, I don't understand that yet. Try asking about tasks, reminders, or quizzes.");
                    break;
            }
        }

        // Open Task Window
        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            if (taskWindow == null)
                taskWindow = new TaskWindow();

            taskWindow.Show();
            BotSay("Task manager opened.");
        }

        // Open Quiz Window
        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
        }

        // Bot response
        private void BotSay(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }

        // Tip generator
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

        // User input parser
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