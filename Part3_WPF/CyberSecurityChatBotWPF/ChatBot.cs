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
using System.Windows.Controls;
using System.Threading;
using System.Text.RegularExpressions; //for type writer effect??

namespace CyberSecurityChatbot
{
    // This class represents the user profile and stores user information
    public static class ChatBot
    {
        // Static instance of UserProfile to store user data
        private static UserProfile user = new UserProfile();

        // Static instance of Random for generating random numbers
        private static Random random = new Random();

        // Static variables to track the last topic discussed and interest tips
        private static string lastUserTopic = "";

        // Dictionary to store interest tips by topic, indexed by interest name
        private static Dictionary<string, int> interestTipIndex = new();

        // To track which interests have already been given a tip this round
        private static HashSet<string> givenTipInterests = new();

        public static TextBlock ChatHistoryTextBlock { get; private set; }

        private static void AppendBotText(string message)
        {
            ChatHistoryTextBlock.Text += $"CyberBot: {message}\n\n";
        }



        //--------------------------------------ASK USERNAME METHOD--------------------------------------//
        // This method initializes the chatbot and asks for the user's name
        public static string AskUsername()

        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //For symbols
            Console.ForegroundColor = ConsoleColor.White;
            ChatHistoryTextBlock.Text += "Whats your name?\n"; //Ask user name
            string name = Console.ReadLine();

            //Error and input handling
            while (string.IsNullOrWhiteSpace(name))
            {
                ChatHistoryTextBlock.Text += "Im sorry I dont quite understand that\n";
                name = Console.ReadLine();
            }

            user.Name = name;

            Console.ForegroundColor = ConsoleColor.Magenta;
            ChatHistoryTextBlock.Text += "Welcome, {name}!\n💻🔒Let's talk cybersecurity. How can I assist?";
            
            Console.ResetColor();

            return name;
        }

        //--------------------------------------GET RESPONSE WITH TOPICS METHOD--------------------------------------------//
        // This method generates a response based on user input and the current topic
        public static (string response, Topic topic) GetResponseWithTopic(string input, Topic currentTopic, string name)
        {

            var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, name);
            if (topic != Topic.None) lastUserTopic = topic.ToString().ToLower();
            return (response, topic);
        }

        //----------------------------------------------------------------------------------------------------------------//

        //-------------------------------------GET ROTATING TIP----------------------------------------------------//
        // This method generates a "rotating tip" which is basically a tip that the bot suggests to user based on 
        // the user's interests, and ir rotates the different types of tips
        private static string GetRotatingTip(string input)
        {
            input = input.ToLower();

            // If the input contains "tip" or "suggestion", it will try to give a tip based on the user's interests
            foreach (string interest in user.Interests)
            {
                // Skip if this interest was already given a tip or if it doesn't match the current topic
                if (givenTipInterests.Contains(interest) || interest != lastUserTopic.ToString().ToLower())
                    continue;

                // Initialize index if not present
                if (!interestTipIndex.ContainsKey(interest))
                    interestTipIndex[interest] = 0;

                // Try to parse interest string to Topic enum if possible
                bool isValidTopic = Enum.TryParse<Topic>(interest, true, out Topic topic);

                if (!isValidTopic || !TipLibrary.TipsByTopic.ContainsKey(topic))
                {
                    // No tips available for this interest, skip to next
                    continue;
                }

                // Get the tips for this topic and select one based on the current index
                string[] tips = TipLibrary.TipsByTopic[topic];
                string tip = tips[interestTipIndex[interest]];

                // Update index to next tip (rotate)
                interestTipIndex[interest] = (interestTipIndex[interest] + 1) % tips.Length;

                // Mark that we gave a tip for this interest this round
                givenTipInterests.Add(interest);

                return $"💡 As someone interested in {interest}, here's a tip: {tip}";
            }

            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------//

        //--------------------------------------START CONVERSATION METHOD--------------------------------------------//
        // This method starts the conversation with the user
        public static void StartConversation(string name)
        {

            Topic currentTopic = Topic.None; // Initialize the current topic to None
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Display a message with instructions
            Console.WriteLine($"\n💬You can ask me things like:" +
                $"\n- How are you?" +
                $"\n- What can I ask you?" +
                $"\n- How do you work?" +
                $"\n- What is phishing?" +
                $"\n- What is a strong password?" +
                $"\n- Tell me about yourself, your age, whether you're a student, professor or engineer and the 2 topics " +
                $"\n you're interested in!" +
                $"\n OR (Type 'exit' to quit.)");
            Console.ResetColor();

            while (true) // Loop until the user types "exit"
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"\n{name}: ");
                string input = Console.ReadLine()?.ToLower();
                Console.ResetColor();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    // Append welcome message
                    AppendBotText($"Welcome, {name}!\n💻🔒Let's talk cybersecurity. How can I assist?");

                    // Append a prompt or prefix label (optional)
                    AppendBotText("CyberBot:");

                    // Append the "Oops! I didn't catch that..." message
                    AppendBotText("Oops! I didn't catch that. Could you try typing something?");
                    continue;
                }

                if (input == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("CyberBot: ");
                    ChatHistoryTextBlock.Text += $"CyberBot: Goodbye, {name}! Stay safe online.\n\n";
                    Console.ResetColor();
                    break;
                }

                // Check for common thank you messages 
                if (input.Contains("thank you") || input.Contains("thanks"))
                {
                    ChatHistoryTextBlock.Text += "CyberBot: You're welcome! 😊 Is there anything else you'd like to know?\n\n";
                    continue;
                }

                // === Start user memory parsing ===
                if (input.StartsWith("i'm ") || input.StartsWith("i am "))
                {
                    ParseUserInfo(input);
                    continue;
                }
                // === End user memory parsing ===

                var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, name);

                // If no response matched, use a fallback
                if (string.IsNullOrWhiteSpace(response))
                {
                    response = "I'm not sure I understand. Could you try rephrasing or asking in a different way?";
                }

                if (topic != Topic.None)
                    currentTopic = topic;

                // Detect sentiment phrase
                string sentiment = SentimentAnalyser.DetectSentiment(input);
                string sentimentResponse = SentimentAnalyser.GetSentimentResponse(sentiment);
                string combinedResponse = !string.IsNullOrEmpty(sentimentResponse) ? sentimentResponse + " " + response : response;

                // Append tip if available
                string? tip = GetRotatingTip(input);
                if (tip != null)
                {
                    combinedResponse += "\n" + tip;
                }

                // Show typing animation before printing the full response
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

                // Print the combined response once
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("CyberBot: ");
                AppendBotText(combinedResponse);
                Console.ResetColor();
            }
        }
        //---------------------------------------------------------------------------------------------//

        //--------------------------------------PARSE USER INFO METHOD--------------------------------------------//
        // This method parses user information from the input string
        private static void ParseUserInfo(string input)
        {
            // Normalize input
            input = input.ToLower();

            bool updated = false; // Flag to check if we got new info

            // Extract age
            var ageMatch = Regex.Match(input, @"(\d{1,3})\s*(year|yr)s?\s*old");
            if (ageMatch.Success && int.TryParse(ageMatch.Groups[1].Value, out int age))
            {
                if (!user.Age.HasValue)
                {
                    user.Age = age;
                    updated = true;
                }
            }

            // Extract role
            if (input.Contains("student") && user.Role != "student") { user.Role = "student"; updated = true; }
            else if (input.Contains("teacher") && user.Role != "teacher") { user.Role = "teacher"; updated = true; }
            else if (input.Contains("engineer") && user.Role != "engineer") { user.Role = "engineer"; updated = true; }

            // Extract interests
            var interestedMatch = Regex.Match(input, @"(?:interested in|learning about)\s+([a-z\s,]+)");
            if (interestedMatch.Success)
            {
                string interestsText = interestedMatch.Groups[1].Value;
                var splitInterests = interestsText.Split(new string[] { ",", " and " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var interest in splitInterests)
                {
                    string trimmed = interest.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !user.Interests.Contains(trimmed))
                    {
                        user.AddInterest(trimmed);
                        updated = true;
                    }
                }
            }

            // Only print message if something was actually updated
            if (updated)
            {
                var ageText = user.Age.HasValue ? user.Age.Value.ToString() : "unknown";
                var interestString = user.Interests.Count > 0 ? string.Join(", ", user.Interests) : "no specific interests";
                AppendBotText($"Thanks for sharing about yourself, {user.Name}! " +
    $"I noted your age as {user.Age ?? 0}, role as {user.Role ?? "unknown"}, " +
    $"and interests in {interestString}.");
                Console.ResetColor();
            }
        }

       
    }
}
//-----------------------------------------------------------END OF PROGRAM--------------------------------------------------------------------------//


