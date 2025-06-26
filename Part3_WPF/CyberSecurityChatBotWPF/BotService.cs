using CyberSecurityChatbot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatBotWPF
{
    public class BotService
    {
        public UserProfile User { get => user; private set => user = value; }
        private Topic currentTopic = Topic.None;
        private Random random = new Random();
        private UserProfile user = new UserProfile();

        public string ProcessInput(string input)
        {
            string lower = input.ToLower();

            // 🔍 Detect "add task" command
            if (lower.StartsWith("add task -") || lower.StartsWith("create task -"))
            {
                string taskTitle = input.Substring(input.IndexOf("-") + 1).Trim();

                // Basic template description
                string description = $"This task is about: {taskTitle}. Remember to complete it soon.";

                // Return a trigger string to MainWindow
                return $"##ADD_TASK##{taskTitle}|{description}";
            }

            // 🔁 Standard chatbot response
            var (response, topic) = ResponseGenerator.GetResponseWithTopic(input, currentTopic, User.Name ?? "friend");
            if (topic != Topic.None) currentTopic = topic;

            if (User.Interests.Count > 0 && random.Next(0, 3) == 0)
            {
                string interest = User.Interests[random.Next(User.Interests.Count)];
                response += $"\n💡 As someone interested in {interest}, you might want to explore that further!";
            }

            return response;
        }

        public void SetUserName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                User.Name = name;
        }

        private string ExtractName(string input)
        {
            string lower = input.ToLower();
            if (lower.StartsWith("my name is"))
                return input.Substring(11).Trim();
            if (lower.StartsWith("i'm"))
                return input.Substring(4).Split(' ')[0]; // crude fallback
            return null;
        }

        private string ExtractInterests(string input)
        {
            if (input.Contains("interested in"))
                return input.Split("interested in")[1].Trim();
            return null;
        }
    }
}