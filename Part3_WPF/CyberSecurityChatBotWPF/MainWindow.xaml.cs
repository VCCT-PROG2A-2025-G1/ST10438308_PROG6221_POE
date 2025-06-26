using CyberSecurityChatbot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CyberSecurityChatBotWPF
{
    public partial class MainWindow : Window
    {
        // Your chatbot's memory and tools
        private static UserProfile user = new UserProfile();
        private static Random random = new Random();
        private static string lastUserTopic = "";
        private static Dictionary<string, int> interestTipIndex = new();
        private static HashSet<string> givenTipInterests = new();

        private Topic currentTopic = Topic.None;

        private TaskWindow taskWindow;

        private CyberTask lastAddedTask = null;

        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
        }

        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            if (taskWindow == null) taskWindow = new TaskWindow();
            taskWindow.Show();
            BotSay("Task manager opened.");
        }

        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (quizWindow == null) quizWindow = new QuizWindow();
            quizWindow.Show();
            BotSay("Quiz started! Answer the questions to test your cybersecurity knowledge.");
        }

        public MainWindow()
        {
            InitializeComponent();
            BotSay("Hello! 👋 Welcome to the Cybersecurity ChatBot.");
            BotSay("What is your name?");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputTextBox.Text.Trim();
            UserInputTextBox.Clear();

            if (string.IsNullOrWhiteSpace(input))
                return;

            ChatHistoryTextBlock.Text += $"You: {input}\n";

            // Add Task Command
            if (input.ToLower().StartsWith("add task"))
            {
                string taskTitle = input.Substring(8).Trim();
                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    BotSay("Sure! What would you like the task to be?");
                    return;  // <-- Return here!
                }

                if (taskWindow == null)
                    taskWindow = new TaskWindow();

                taskWindow.Show();

                taskWindow.TitleBox.Text = taskTitle;

                lastAddedTask = new CyberTask { Title = taskTitle };
                taskWindow.Tasks.Add(lastAddedTask);
                taskWindow.RefreshList();

                BotSay($"Task added with title \"{taskTitle}\". You can add more details or a reminder in the task window, or tell me to remind you in X days.");
                return;  // <-- Return here!
            }

            // Remind me in X days
            var remindMatch = Regex.Match(input.ToLower(), @"remind me in (\d+) days");
            if (remindMatch.Success && lastAddedTask != null)
            {
                if (int.TryParse(remindMatch.Groups[1].Value, out int days))
                {
                    lastAddedTask.ReminderDate = DateTime.Today.AddDays(days);
                    taskWindow?.RefreshList();

                    BotSay($"Got it! I'll remind you about \"{lastAddedTask.Title}\" in {days} days.");
                    return;  // <-- Return here!
                }
            }

            // Default fallback (only reached if no other conditions matched)
            BotSay("Sorry, I don't understand that yet. Try asking about:\nPhishing\nMalware\nPasswords\nFirewalls\nScams\nPrivacy\nEncryption\n2FA\nVPNs\no Phishing tips\n· Password tips");
        }

        private void AppendUser(string message)
        {
            ChatHistoryTextBlock.Text += $"You: {message}\n";
        }

        private void BotSay(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }

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

                return $"As someone interested in {interest}, here's a tip: {tip}";
            }

            return null;
        }

        private static void ParseUserInfo(string input)
        {
            input = input.ToLower();
            bool updated = false;

            var ageMatch = System.Text.RegularExpressions.Regex.Match(input, @"(\d{1,3})\s*(year|yr)s?\s*old");
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

            var interestMatch = System.Text.RegularExpressions.Regex.Match(input, @"(?:interested in|learning about)\s+([a-z\s,]+)");
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
                // In console this would be styled — here just show text
                MessageBox.Show(summary, "User Info Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}