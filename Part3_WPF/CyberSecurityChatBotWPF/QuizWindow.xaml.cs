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
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        private List<QuizQuestion> Questions = new();
        private int currentQuestionIndex = 0;
        private int score = 0;

        public QuizWindow()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion();
        }

        private void LoadQuestions()
        {
            // At least 10 questions 
            Questions = new List<QuizQuestion>();

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectAnswerIndex = 2,
                Explanation = "Correct! Reporting phishing emails helps prevent scams."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: You should use the same password for all your accounts.",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 1,
                Explanation = "False! Using unique passwords for each account keeps you safer."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What is the purpose of two-factor authentication (2FA)?",
                Options = new List<string> { "To make passwords longer", "To add an extra layer of security", "To encrypt your data", "To block spam emails" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! 2FA adds an extra layer of security by requiring a second form of verification."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What is a VPN used for?",
                Options = new List<string> { "To speed up your internet", "To hide your IP address and encrypt your traffic", "To block ads", "To download files faster" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! A VPN hides your IP address and encrypts your internet traffic."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What is phishing?",
                Options = new List<string> { "A type of malware", "A method to steal personal information", "A way to speed up your computer", "A type of firewall" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! Phishing is a method used by attackers to trick you into giving them your personal information."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What is malware?",
                Options = new List<string> { "A type of antivirus", "Software designed to harm your computer", "A type of firewall", "A secure way to store passwords" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! Malware is software designed to harm your computer or steal your information."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "What is the best way to create a strong password?",
                Options = new List<string> { "Use your name and birthdate", "Use a mix of letters, numbers, and symbols", "Use the same password for all accounts", "Write it down on a sticky note" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! A strong password uses a mix of letters, numbers, and symbols."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "Which of the following may indicate your device is infected with malware?",
                Options = new List<string> { "It runs slightly slower", "You see frequent pop-ups", "Your battery lasts longer", "Your screen is brighter" },
                CorrectAnswerIndex = 1,
                Explanation = "Correct! Pop-ups and performance issues may indicate malware infection."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: A green padlock in the browser means the website is completely safe.",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 1,
                Explanation = "False! A padlock only means the connection is encrypted — not that the site itself is safe."
            });

            Questions.Add(new QuizQuestion
            {
                QuestionText = "Which of the following is an example of social engineering?",
                Options = new List<string> { "A virus that spreads through email", "An app that tracks your location", "An attacker tricking you into revealing personal info", "A firewall blocking your traffic" },
                CorrectAnswerIndex = 2,
                Explanation = "Correct! Social engineering relies on manipulating people into giving up confidential information."
            });
        }

        private void DisplayQuestion()
        {
            FeedbackTextBlock.Text = "";
            ScoreTextBlock.Text = $"Score: {score}/{currentQuestionIndex}";

            if (currentQuestionIndex >= Questions.Count)
            {
                // Quiz finished
                QuestionTextBlock.Text = $"Quiz completed! Your score: {score}/{Questions.Count}";
                AnswersPanel.Children.Clear();
                NextButton.IsEnabled = false;

                string feedback = score > Questions.Count * 0.7
                    ? "Great job! You're a cybersecurity pro!"
                    : "Keep learning to stay safe online!";
                FeedbackTextBlock.Text = feedback;
                return;
            }

            QuizQuestion q = Questions[currentQuestionIndex];
            QuestionTextBlock.Text = q.QuestionText;

            AnswersPanel.Children.Clear();

            for (int i = 0; i < q.Options.Count; i++)
            {
                RadioButton rb = new RadioButton
                {
                    Content = q.Options[i],
                    GroupName = "Answers",
                    Tag = i
                };
                AnswersPanel.Children.Add(rb);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int? selectedIndex = null;
            foreach (RadioButton rb in AnswersPanel.Children)
            {
                if (rb.IsChecked == true)
                {
                    selectedIndex = (int)rb.Tag;
                    break;
                }
            }

            if (selectedIndex == null)
            {
                MessageBox.Show("Please select an answer before continuing.");
                return;
            }

            QuizQuestion q = Questions[currentQuestionIndex];
            if (selectedIndex == q.CorrectAnswerIndex)
            {
                score++;
                FeedbackTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                FeedbackTextBlock.Text = "Correct! " + q.Explanation;
            }
            else
            {
                FeedbackTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                FeedbackTextBlock.Text = "Incorrect. " + q.Explanation;
            }

            currentQuestionIndex++;
            ScoreTextBlock.Text = $"Score: {score}/{currentQuestionIndex}";

            if (currentQuestionIndex < Questions.Count)
            {
                // Wait 2 seconds then display next question
                NextButton.IsEnabled = false;
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = System.TimeSpan.FromSeconds(2);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    DisplayQuestion();
                    NextButton.IsEnabled = true;
                };
                timer.Start();
            }
            else
            {
                QuestionTextBlock.Text = $"Quiz completed! Your score: {score}/{Questions.Count}";
                AnswersPanel.Children.Clear();
                NextButton.IsEnabled = false;

                string feedback = score > Questions.Count * 0.7
                    ? "Great job! You're a cybersecurity pro!"
                    : "Keep learning to stay safe online!";
                FeedbackTextBlock.Text = feedback;
            }
        }
    }
}
