using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CyberSecurityChatbot; 

namespace CyberSecurityChatBotWPF
{
    //Using the logic from part 1 and part 2, this class handles the chatbot's functionality
    public class ChatbotService
    {
        private UserProfile user = new UserProfile();
        private Topic currentTopic = Topic.None;
        private string lastUserTopic = "";
        private Dictionary<string, int> interestTipIndex = new();
        private HashSet<string> givenTipInterests = new();
        private Random random = new();

        // Ask for username (initial prompt)
        public string AskUsernamePrompt()
        {
            return "What is your name?";
        }

        // Process username input and save it
        public string SetUsername(string input)
        {
            input = input?.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return "Please enter a valid name:";

            user.Name = input;
            return $"Welcome, {user.Name}! 💻🔒 Let's talk cybersecurity. How can I assist?";
        }

        // Main method to process user input and generate a chatbot response
        public async Task<string> GetResponseAsync(string input)
        {
            input = input?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(input))
                return "Oops! I didn't catch that. Could you try typing something?";

            if (input == "exit")
                return $"Goodbye, {user.Name}! Stay safe online.";

            if (input.Contains("thank you") || input.Contains("thanks"))
                return "You're welcome! 😊 Is there anything else you'd like to know?";

            // Parse user info (age, role, interests) if they mention it
            if (input.StartsWith("i'm ") || input.StartsWith("i am "))
            {
                return ParseUserInfo(input);
            }

            // Generate response based on current topic and input
            var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, user.Name);

            if (topic != Topic.None)
            {
                currentTopic = topic;
                lastUserTopic = topic.ToString().ToLower();
            }

            // Append rotating tip if user requests or tip is available
            string tip = GetRotatingTip(input);
            if (!string.IsNullOrEmpty(tip))
            {
                response += "\n" + tip;
            }

            return response;
        }

        // Parses user information from input and updates the user profile
        private string ParseUserInfo(string input)
        {
            bool updated = false;

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

            if (updated)
            {
                var ageText = user.Age.HasValue ? user.Age.Value.ToString() : "unknown";
                var interestString = user.Interests.Count > 0 ? string.Join(", ", user.Interests) : "no specific interests";
                return $"Thanks for sharing about yourself, {user.Name}! I noted your age as {ageText}, role as {user.Role ?? "unknown"}, and interests in {interestString}.";
            }

            return "Thanks for sharing!";
        }

        // Retrieves a rotating tip based on user interests, avoiding repetition
        private string GetRotatingTip(string input)
        {
            input = input.ToLower();

            foreach (string interest in user.Interests)
            {
                if (givenTipInterests.Contains(interest) || interest != lastUserTopic)
                    continue;

                if (!interestTipIndex.ContainsKey(interest))
                    interestTipIndex[interest] = 0;

                bool isValidTopic = Enum.TryParse<Topic>(interest, true, out Topic topic);

                if (!isValidTopic || !TipLibrary.TipsByTopic.ContainsKey(topic))
                    continue;

                string[] tips = TipLibrary.TipsByTopic[topic];
                string tip = tips[interestTipIndex[interest]];

                interestTipIndex[interest] = (interestTipIndex[interest] + 1) % tips.Length;
                givenTipInterests.Add(interest);

                return $"💡 As someone interested in {interest}, here's a tip: {tip}";
            }

            return null;
        }
    }
}
//------------------------------------------------END OF FILE-----------------------------------------------------//
