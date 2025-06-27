using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace CyberSecurityChatBotWPF
{

    // -----------------------------------Event arguments for quiz completion-------------------------//
    public class QuizCompletedEventArgs : EventArgs
    {
        public int Score { get; }
        public int TotalQuestions { get; }
        public double Percentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
        public List<string> QuestionsAnswered { get; }

        
        public QuizCompletedEventArgs(int score, int totalQuestions, List<string> questionsAnswered)
        {
            Score = score;
            TotalQuestions = totalQuestions;
            QuestionsAnswered = questionsAnswered;
        }
    }

    
    public partial class QuizWindow : Window
    {
        private List<QuizQuestion> Questions = new();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private List<string> answeredQuestions = new();
        private List<int> CorrectlyAnsweredIndices = new(); // ✅ moved here

        public static event EventHandler<QuizCompletedEventArgs> QuizCompleted;

        public QuizWindow()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion();
        }

        private void LoadQuestions()
        {
            Questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "What should you do if you receive an email asking for your password?",
                    Options = new() { "Reply with password", "Delete the email", "Report as phishing", "Ignore it" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Reporting phishing emails helps protect others too."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: You should use the same password for all your accounts.",
                    Options = new() { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "False, Reusing passwords makes all accounts vulnerable."
                },
                new QuizQuestion
                {
                    QuestionText = "What is the purpose of two-factor authentication (2FA)?",
                    Options = new() { "Encrypt data", "Extra security layer", "Make passwords longer", "Prevent spam" },
                    CorrectAnswerIndex = 1,
                    Explanation = "2FA adds an extra step to verify your identity."
                },
                new QuizQuestion
                {
                    QuestionText = "What is a VPN used for?",
                    Options = new() { "Hide IP & encrypt traffic", "Speed up internet", "Block ads", "Download faster" },
                    CorrectAnswerIndex = 0,
                    Explanation = "VPNs enhance privacy and protect online activity."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is an example of social engineering?",
                    Options = new() { "Virus through email", "Tricking you for info", "Firewall blocking", "Location tracking app" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Social engineering manipulates people to gain access."
                },
                new QuizQuestion
                {
                    QuestionText = "What does VPN stand for?",
                    Options = new() { "Virtual Private Network", "Very Personal Network", "Verified Public Network", "Virtual Public Node" },
                    CorrectAnswerIndex = 0,
                    Explanation = "VPN creates a secure, encrypted connection over the internet."
                },
                new QuizQuestion
                {
                    QuestionText = "What is social engineering in cybersecurity?",
                    Options = new() { "Hacking software", "Manipulating people to reveal info", "A type of firewall", "Encrypting data" },
                    CorrectAnswerIndex = 1,
                    Explanation = "It’s the art of tricking people to gain confidential information."
                },
                new QuizQuestion
                {
                    QuestionText = "What is the safest way to store passwords?",
                    Options = new() { "Write them down", "Use a password manager", "Use the same password everywhere", "Save in a text file" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Password managers securely store and generate complex passwords."
                },
                new QuizQuestion
                {
                    QuestionText = "What is ransomware?",
                    Options = new() { "Software that blocks ads", "Malware that locks files until a ransom is paid", "A security update", "A virus that speeds up your PC" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Ransomware encrypts your data and demands payment for the key."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of the following can help protect against phishing?",
                    Options = new() { "Using two-factor authentication", "Ignoring emails", "Sharing passwords", "Clicking all links" },
                    CorrectAnswerIndex = 0,
                    Explanation = "2FA adds security even if your password is compromised."
                }
            };
        }

        private void DisplayQuestion()
        {

            ScoreTextBlock.Text = $"Score: {score}/{currentQuestionIndex}";

            if (currentQuestionIndex >= Questions.Count)
            {
                QuestionTextBlock.Text = $"Quiz completed! Your score: {score}/{Questions.Count}";
                AnswersPanel.Children.Clear();
                NextButton.IsEnabled = false;

                double percentage = (double)score / Questions.Count;

                if (percentage >= 0.7)
                {
                    FeedbackTextBlock.Foreground = Brushes.Green;
                    FeedbackTextBlock.Text = "🎉 Great job! You're a cybersecurity pro!";
                }
                else
                {
                    FeedbackTextBlock.Foreground = Brushes.Red;
                    FeedbackTextBlock.Text = "📘 Keep learning! Review topics to improve your cyber safety.";
                }

                var incorrectQuestions = Questions
                    .Where((q, i) => !CorrectlyAnsweredIndices.Contains(i))
                    .Select(q => $"❌ {q.QuestionText} — Correct: {q.Options[q.CorrectAnswerIndex]}")
                    .ToList();

                if (incorrectQuestions.Any())
                {
                    FeedbackTextBlock.Text += "\n\nQuestions you missed:\n" + string.Join("\n", incorrectQuestions);
                }

                QuizCompleted?.Invoke(this, new QuizCompletedEventArgs(score, Questions.Count, answeredQuestions));
                return;
            }

            var question = Questions[currentQuestionIndex];
            QuestionTextBlock.Text = question.QuestionText;
            AnswersPanel.Children.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                RadioButton rb = new RadioButton
                {
                    Content = question.Options[i],
                    GroupName = "Answers",
                    Tag = i
                };
                AnswersPanel.Children.Add(rb);
            }
        }

        private void LogIncorrectAnswer(QuizQuestion question, int selectedIndex)
        {
            string selectedText = question.Options[selectedIndex];
            string correctText = question.Options[question.CorrectAnswerIndex];
            string log = $"❌ Incorrect: \"{question.QuestionText}\" | You chose: \"{selectedText}\" | Correct: \"{correctText}\"";

            Console.WriteLine(log);

            
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.LogActivity("[Quiz] " + log);
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int? selected = null;
            foreach (RadioButton rb in AnswersPanel.Children)
            {
                if (rb.IsChecked == true)
                {
                    selected = (int)rb.Tag;
                    break;
                }
            }

            if (selected == null)
            {
                MessageBox.Show("Please select an answer.");
                return;
            }

            var q = Questions[currentQuestionIndex];
            answeredQuestions.Add(q.QuestionText);

            if (selected == q.CorrectAnswerIndex)
            {
                score++;
                CorrectlyAnsweredIndices.Add(currentQuestionIndex);
                FeedbackTextBlock.Foreground = Brushes.Green;
                FeedbackTextBlock.Text = "✅ Correct! " + q.Explanation;
            }
            else
            {
                FeedbackTextBlock.Foreground = Brushes.Red;
                FeedbackTextBlock.Text = "❌ Incorrect. " + q.Explanation;

                LogIncorrectAnswer(q, selected.Value);
            }

            currentQuestionIndex++;

            if (currentQuestionIndex < Questions.Count)
            {
                NextButton.IsEnabled = false;
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Tick += (s, _) =>
                {
                    timer.Stop();
                    FeedbackTextBlock.Text = ""; // ✅ Clear it right before loading next question
                    DisplayQuestion();
                    NextButton.IsEnabled = true;
                };
                timer.Start();
            }
            else
            {
                DisplayQuestion(); 
            }
        }

        public class QuizQuestion
        {
            public string QuestionText { get; set; }
            public List<string> Options { get; set; }
            public int CorrectAnswerIndex { get; set; }
            public string Explanation { get; set; }
        }
    }
}
//----------------------------------------------------END OF PROGRAM----------------------------------------------------//
