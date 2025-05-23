//Funi Mapunda
//ST10438308
//BCAD2 GR1

//References: 
//codecademy.com
//stackoverflow.com
//chatgpt.com

using System;
using System.Xml.Linq;
using System.Media; // For WAV playback
using NAudio.Wave;
using System.Threading;
using System.Text.RegularExpressions; //for type writer effect??

namespace CyberSecurityChatbot
{
    public static class ChatBot
    {
        private static UserProfile user = new UserProfile();
        private static Random random = new Random();
        private static string lastUserTopic = "";
        private static Dictionary<string, int> interestTipIndex = new();
        private static HashSet<string> givenTipInterests = new();

        // This method initializes the chatbot and asks for the user's name


        public static string AskUsername()

        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //For symbols
            Console.ForegroundColor = ConsoleColor.White;
            ChatBotUI.PrintTyping("What is your name?", ConsoleColor.White, 30);
            string name = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(name))
            {
                ChatBotUI.PrintTyping("Please enter a valid name:", ConsoleColor.Yellow, 25);
                name = Console.ReadLine();
            }

            user.Name = name;

            Console.ForegroundColor = ConsoleColor.Magenta;
            ChatBotUI.PrintTyping($"Welcome, {name}!\n💻🔒Let's talk cybersecurity. How can I assist? ", ConsoleColor.Magenta, 25);
            Console.ResetColor();

            return name;
        }

        // This method generates a response based on user input and the current topic
        public static (string response, Topic topic) GetResponseWithTopic(string input, Topic currentTopic, string name)
        {
            var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, name);
            if (topic != Topic.None) lastUserTopic = topic.ToString().ToLower();
            return (response, topic);
        }

        // This method generates a rotating tip based on the user's interests

        private static string GetRotatingTip(string input)
        {
            input = input.ToLower();

            foreach (string interest in user.Interests)
            {
                // Only give tip if interest matches the current topic being discussed
                if (givenTipInterests.Contains(interest) || interest != lastUserTopic)
                    continue;

                if (!interestTipIndex.ContainsKey(interest))
                    interestTipIndex[interest] = 0;

                string[] tips = interest switch
                {
                    "malware" => new[]
                    {
                "Keep your software updated to patch malware vulnerabilities.",
                "Avoid pirated software—it’s a common malware source.",
                "Regularly back up your files in case of ransomware attacks."
            },
                    "phishing" => new[]
                    {
                "Check the sender’s email address carefully—it might be spoofed.",
                "Hover over links to preview URLs before clicking.",
                "Enable two-factor authentication for extra protection."
            },
                    _ => new[] { $"Stay informed about {interest}—read trusted blogs and news sources." }
                };

                string tip = tips[interestTipIndex[interest]];
                interestTipIndex[interest] = (interestTipIndex[interest] + 1) % tips.Length;
                givenTipInterests.Add(interest);
                return $"💡 As someone interested in {interest}, here's a tip: {tip}";
            }

            return null;
        }

        // This method starts the conversation with the user

        public static void StartConversation(string name)
        {

            Topic currentTopic = Topic.None;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n💬You can ask me things like:" +
                $"\n- How are you?" +
                $"\n- What can I ask you?" +
                $"\n- How do you work?" +
                $"\n- What is phishing?" +
                $"\n- What is a strong password?" +
                $"\n- Tell me about yourself, your age, whether you're a student, professor or engineer and the 2 topics you're interested in!" +
                $"\n OR (Type 'exit' to quit.)");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"\n{name}: ");
                string input = Console.ReadLine()?.ToLower();
                Console.ResetColor();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    ChatBotUI.PrintTyping("CyberBot: ", ConsoleColor.Magenta, 30);
                    ChatBotUI.PrintTyping("Oops! I didn't catch that. Could you try typing something?\n", ConsoleColor.White, 25);
                    Console.ResetColor();
                    continue;
                }

                if (input == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("CyberBot: ");
                    ChatBotUI.PrintTyping($" Goodbye, {name}! Stay safe online. ", ConsoleColor.Yellow, 30);
                    Console.ResetColor();
                    break;
                }

                // === New user memory parsing starts here ===
                if (input.StartsWith("i'm ") || input.StartsWith("i am "))
                {
                    ParseUserInfo(input);
                    continue;
                }
                // === End user memory parsing ===

                var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, name);
                if (topic != Topic.None)
                    currentTopic = topic;

                int startLine = Console.CursorTop;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("CyberBot is typing");
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(400);
                }
                Thread.Sleep(500);
                Console.SetCursorPosition(0, startLine);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, startLine);

                string? tip = GetRotatingTip(input);
                if (tip != null)
                {
                    response += "\n" + tip;
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine();
                Console.Write("CyberBot: ");
                ChatBotUI.PrintTyping(response, ConsoleColor.Magenta, 35);
                Console.ResetColor();
            }
        }

        private static void ParseUserInfo(string input)
        {
            // Normalize input
            input = input.ToLower();

            // Extract age - look for pattern like "21 year old" or "21 yrs old"
            var ageMatch = Regex.Match(input, @"(\d{1,3})\s*(year|yr)s?\s*old");
            if (ageMatch.Success && int.TryParse(ageMatch.Groups[1].Value, out int age))
            {
                user.Age = age;
            }

            // Extract role/student status
            if (input.Contains("student")) user.Role = "student";
            else if (input.Contains("teacher")) user.Role = "teacher";
            else if (input.Contains("engineer")) user.Role = "engineer";

            // Extract interests - look for "interested in" or "learning about"
            var interests = new List<string>();

            // First, try to find "interested in X" or "learning about X"
            var interestedMatch = Regex.Match(input, @"(?:interested in|learning about)\s+([a-z\s,]+)");
            if (interestedMatch.Success)
            {
                string interestsText = interestedMatch.Groups[1].Value;
                // Split by commas or "and"
                var splitInterests = interestsText.Split(new string[] { ",", " and " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var interest in splitInterests)
                {
                    string trimmed = interest.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        user.AddInterest(trimmed);
                }
            }

            // Confirmation message
            var ageText = user.Age.HasValue ? user.Age.Value.ToString() : "unknown";
            var interestString = user.Interests.Count > 0 ? string.Join(", ", user.Interests) : "no specific interests";
            ChatBotUI.PrintTyping($"CyberBot: Thanks for sharing about yourself, {user.Name}! " +
                $"I noted your age as {user.Age ?? 0}, role as {user.Role ?? "unknown"}, " +
                $"and interests in {interestString}.\n", ConsoleColor.Magenta, 30);
        }
    }
}
