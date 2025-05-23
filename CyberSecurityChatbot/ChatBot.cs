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
using System.Threading; //for type writer effect??

namespace CyberSecurityChatbot
{
    public static class ChatBot
    {

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

            Console.ForegroundColor = ConsoleColor.Magenta;
            ChatBotUI.PrintTyping($"Welcome, {name}!\n💻🔒Let's talk cybersecurity. How can I assist? ", ConsoleColor.Magenta, 25);
            Console.ResetColor();

            return name;
        }

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
                $"\n- Or anything related to cybersecurity (scams, encryption etc)" +
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

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine();
                Console.Write("CyberBot: ");
                ChatBotUI.PrintTyping(response, ConsoleColor.Magenta, 35);
                Console.ResetColor();
            }
        }
    }
}