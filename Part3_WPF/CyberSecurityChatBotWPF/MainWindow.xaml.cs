//Funi Mapunda
//ST10438308
//BCAD2 GR1

//References: 
//codecademy.com
//stackoverflow.com
//chatgpt.com
//https://medium.com/@thomasmatlockbba/exploring-natural-language-processing-with-c-from-basics-to-advanced-techniques-9114f7da802a

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;


namespace CyberSecurityChatBotWPF
{
    // ------------------------Main window class for the Cyber Security Chat Bot application--------------------------//
    public partial class MainWindow : Window
    {
        // Static reference to the main window instance
        public static MainWindow Instance { get; private set; }

        // Instance of the TaskWindow to manage tasks
        private TaskWindow taskWindow;

        // Last added task to allow setting reminders
        private CyberTask lastAddedTask;

        // Activity log to keep track of user interactions
        private List<string> activityLog = new List<string>();

        // Current page for activity log pagination
        private int activityLogPage = 0;
        private const int ActivityLogPageSize = 5;

        // Instance of the chatbot service to handle user interactions
        private ChatbotService chatbotService = new ChatbotService();

        // Flag to check if we are awaiting the username input
        private bool awaitingUsername = true;

        //-------------------------CONSTRUCTOR-------------------------`//
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();

            BotSay(chatbotService.AskUsernamePrompt());
            // Subscribe to quiz event
            QuizWindow.QuizCompleted += OnQuizCompleted;

            InitializeComponent();
            Instance = this; // ✅ Set the static reference here
        }


        //-------------------------EVENT HANDLERS-------------------------//
        // Handles the Send button click event
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUser(input);
            UserInputTextBox.Clear();

            if (awaitingUsername)
            {
                string welcomeMsg = chatbotService.SetUsername(input);
                awaitingUsername = false;
                BotSay(welcomeMsg);
                return;
            }

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
                    activityLogPage = 0;
                    ShowActivityLog();
                    break;

                case UserIntent.ShowMoreLog:
                    activityLogPage++;
                    ShowActivityLog();
                    break;

                case UserIntent.OpenTaskManager:
                    OpenTaskManager();
                    break;

                case UserIntent.Unknown:
                default:
                    string botResponse = await chatbotService.GetResponseAsync(input);
                    BotSay(botResponse);
                    break;
            }

            if (input.ToLower() == "exit")
            {
                UserInputTextBox.IsEnabled = false;
                SendButton.IsEnabled = false;
            }
        }


        // Handles the Task Manager button click event
        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            EnsureTaskWindow();
            taskWindow.Show();
            BotSay("Task Manager opened.");
        }

        // Handles the Quiz button click event
        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
            BotSay("Cybersecurity quiz started.");
        }

        // Handles adding a new task
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

        // Handles quiz completion event
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

        // Handles setting a reminder for a task
        private void HandleSetReminder(string text, int? days)
        {
            text = text?.Trim();

            // If no new task text but days exist, set reminder on last added task
            if (days.HasValue && string.IsNullOrWhiteSpace(text) && lastAddedTask != null)
            {
                lastAddedTask.ReminderDate = DateTime.Today.AddDays(days.Value);
                taskWindow?.RefreshList();

                BotSay($"Reminder set for \"{lastAddedTask.Title}\" in {days.Value} days.");
                LogActivity($"[Reminder] Reminder set for \"{lastAddedTask.Title}\" in {days.Value} days");
                LogActivity("[Topic] Detected conversation topic: Reminder");
                return;
            }

            // If no task text, ask for it
            if (string.IsNullOrWhiteSpace(text))
            {
                BotSay("What should I remind you about?");
                return;
            }

            // Create a new task with optional reminder
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

        // Shows the activity log, paginated
        private void ShowActivityLog()
        {
            if (activityLog.Count == 0)
            {
                BotSay("Activity log is empty.");
                return;
            }

            // Calculate which slice of log entries to show
            int skip = activityLogPage * ActivityLogPageSize;
            var pageEntries = activityLog.Skip(skip).Take(ActivityLogPageSize).ToList();

            if (pageEntries.Count == 0)
            {
                BotSay("No more activity log entries.");
                return;
            }

            string message = $"Showing activity log entries {skip + 1} to {skip + pageEntries.Count} out of {activityLog.Count}:\n";
            message += string.Join("\n", pageEntries.Select((entry, idx) => $"{skip + idx + 1}. {entry}"));

            BotSay(message);

            // Suggest "show more" if more entries exist
            if (skip + ActivityLogPageSize < activityLog.Count)
                BotSay("Type 'show more log' to see more entries.");
        }

        // Logs an activity with a timestamp
        public void LogActivity(string actionDescription)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            activityLog.Add($"[{timestamp}] {actionDescription}");
        }

        // Shows more activity log entries
        private void ShowMoreActivityLog()
        {
            activityLogPage++;
            ShowActivityLog();
        }

        // Appends a message from the bot to the chat history
        private void BotSay(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }

        // Appends a message from the user to the chat history
        private void AppendUser(string input)
        {
            ChatHistoryTextBlock.Text += $"You: {input}\n";
        }

        // Ensures the task window is created and visible
        private void EnsureTaskWindow()
        {
            if (taskWindow == null || !taskWindow.IsVisible)
                taskWindow = new TaskWindow();
        }

        // Opens the task manager window
        private void OpenTaskManager()
        {
            EnsureTaskWindow();
            taskWindow.Show();
            BotSay("Task manager opened.");
        }

        // Opens the quiz window
        private void OpenQuiz_Click()
        {
            new QuizWindow().Show();
        }

        // Enum to represent user intents
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

        // Detects the user's intent based on input text

        private (UserIntent, string, int?) DetectUserIntent(string input)
        {
            input = input.ToLower();

            // Open task manager
            if (input.Contains("task manager"))
                return (UserIntent.OpenTaskManager, "", null);

            // Start quiz
            if (input.Contains("start quiz") || input.Contains("quiz me"))
                return (UserIntent.StartQuiz, "", null);

            // Show activity log
            if (input.Contains("show activity") || input.Contains("what have you done"))
                return (UserIntent.ShowLog, "", null);

            if (input.Contains("show more log") || input.Contains("more log"))
                return (UserIntent.ShowMoreLog, "", null);

            // Add task pattern: "add task [task description]"
            var addMatch = Regex.Match(input, @"(?:add|create)(?: a)? task(?: to)? (.+)");
            if (addMatch.Success)
                return (UserIntent.AddTask, addMatch.Groups[1].Value.Trim(), null);

            // Reminder patterns:
            if (input.Contains("remind me") || input.Contains("set a reminder"))
            {
                // Pattern: remind me to [task] in [N] days
                var match = Regex.Match(input, @"remind me to (.+?) in (\d+) days");
                if (match.Success)
                {
                    string taskText = match.Groups[1].Value.Trim();
                    int days = int.Parse(match.Groups[2].Value);
                    return (UserIntent.SetReminder, taskText, days);
                }

                // Pattern: remind me to [task] (without days)
                var fallback = Regex.Match(input, @"remind me to (.+)");
                if (fallback.Success)
                {
                    return (UserIntent.SetReminder, fallback.Groups[1].Value.Trim(), null);
                }

                return (UserIntent.SetReminder, "", null);
            }

            // "yes, remind me in N days" to confirm setting reminder on last task
            var yesRemind = Regex.Match(input, @"yes.*remind.*in (\d+) days");
            if (yesRemind.Success)
            {
                int days = int.Parse(yesRemind.Groups[1].Value);
                return (UserIntent.SetReminder, "", days);
            }

            // Default unknown intent
            return (UserIntent.Unknown, "", null);
        }

    }
}
//-------------------------------------------------------END OF FILE-------------------------------------------------------//

















