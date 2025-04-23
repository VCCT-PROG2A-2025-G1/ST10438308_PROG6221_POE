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
    class Program
    {
        //------------------------------------------Main Method--------------------------------------//
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //For symbols

            Console.Title = "🌸 CyberSecurity Chatbot 🌸"; //Tab title

            //Program structure
            PlayGreetingAudioAsync();
            DisplayGreeting();
            
            // Extra: Wait for the greeting to finish before asking the name
            await Task.Delay(1000); // Delay before asking the name
            string name = AskUsername();

            StartConversation(name);
        }
        //-----------------------------------------------------------------------------------------//


        //------------------------------------Play Audio Method---------------------------------------//
        static async Task PlayGreetingAudioAsync()
        {
            string audioFilePath = "chatbotgreetings.wav"; // Path to greeting WAV file

            // Play the audio in a separate task (asynchronously)
            await Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(audioFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100); // Keep the application alive until the audio finishes
                    }
                }
            });
        }
        //--------------------------------------------------------------------------------------------//


        //-------------------------------------------------------------Display Greeting Method----------------------------------------------------//
        static void DisplayGreeting()
        {
            
            //Header
            Console.BackgroundColor = ConsoleColor.Black; // Keeps contrast readable
            Console.ForegroundColor = ConsoleColor.Magenta; // Soft pink tone
            Console.WriteLine(@"
 ═════════════════════════════════════════════════════
           🌸 CYBERSECURITY AWARENESS BOT  🌸     
 ═════════════════════════════════════════════════════");

            //ASCII Cat Logo
            // Save cursor position before drawing the cat
            int catTop = Console.CursorTop; // Track where the cat starts on the console
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"******");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"/\_/\");
            Console.WriteLine(@"(o.o)");
            Console.WriteLine(@" >^<");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"******");

            //Welcome Message
            PrintTyping("👋Hey there! \nWelcome to the Cybersecurity Awareness Bot. I’m here to help you stay safe online.", ConsoleColor.White, 50);
            Console.WriteLine(@"---------------------------------------------------------------------------------------------------------------------");


            Console.ResetColor();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------//


        //------------------------------------Get User Name Method-----------------------------------//
        static string AskUsername()
        {
            //User input
            Console.ForegroundColor = ConsoleColor.White;
            PrintTyping("What is your name?", ConsoleColor.White, 30);
            string name = Console.ReadLine();

            //Personalisation
            Console.ForegroundColor = ConsoleColor.Magenta;
            PrintTyping($"Welcome, {name}!\n💻🔒Let's talk cybersecurity. How can I assist?", ConsoleColor.Magenta,25);
            Console.ResetColor();

            return name;


        }

        //--------------------------------------------------------------------------------------------//


        //---------------------------------------------------Conversation Method------------------------------------------------//
        static void StartConversation(string name)
        {
            //Menu of topics to ask
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n💬You can ask me things like:" +
                $"\n- How are you?" +
                $"\n- What can I ask you?" +
                $"\n- How do you work?" +
                $"\n- What is phishing?" +
                $"\n- What is a strong password?" +
                $"\n OR (Type 'exit' to quit.)");
            Console.ResetColor();

            //Conversation Logic Loop
            while (true)
            {
                //User input question
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"\n{name}: ");
                string input = Console.ReadLine()?.ToLower();
                Console.ResetColor();

                //Exception Handling
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    PrintTyping("CyberBot: ", ConsoleColor.Magenta, 30);
                    PrintTyping("Oops! I didn't catch that. Could you try typing something?\n", ConsoleColor.White, 25);
                    Console.ResetColor();
                    continue;
                }

                //Exit application
                if (input == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("CyberBot: ");
                    PrintTyping($" Goodbye, {name}! Stay safe online. ", ConsoleColor.Yellow, 30);
                    Console.ResetColor();
                    break;
                }

                
                //Response variable
                string response = GetResponse(input, name);

                //Show chatbot typing indicator
                int startLine = Console.CursorTop;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("CyberBot is typing");
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(400);
                }
                Thread.Sleep(500);

                //Erase the typing line for smoothness
                Console.SetCursorPosition(0, startLine);
                Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
                Console.SetCursorPosition(0, startLine); // Set cursor back to start

                //Show CyberBot response
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine();
                Console.Write("CyberBot: ");
                PrintTyping(response, ConsoleColor.Magenta, 35);
                Console.ResetColor();
            }
            //---------------------------------------------------------------------------------------------------------//


            //-----------------Encapsulation method: GetResponse to keep logic----------------------------//
            static string GetResponse(string input, string name)
            {
                // Friendly responses using string manipulation
                if (input.Contains("how are you"))
                    return $"I'm great, {name}! Always ready to fight cyber threats.";

                if (input.Contains("what can i ask"))
                    return "You can ask anything about phishing, malware, passwords, firewalls, and about how to stay safe online!";

                if (input.Contains("how do you work"))
                    return "I use simple keyword recognition to give you answers. I'm not AI, but I do try my best!";

                // Cybersecurity topics
                if (input.Contains("phishing"))
                    return "Phishing is when attackers try to trick you into sharing sensitive info using fake emails or links.";

                if (input.Contains("malware"))
                    return "Malware is software designed to damage your system. Use antivirus and be cautious of suspicious files.";

                if (input.Contains("password"))
                    return "Use complex passwords with upper/lowercase letters, symbols, and numbers. Avoid using personal details, \nlike your birthday or favourite number.";

                if (input.Contains("firewall"))
                    return "A firewall monitors traffic to block dangerous connections. It's your first line of digital defense.";

                //Exception Handling
                return $"Sorry {name}, I don’t understand that yet. Try asking about:\\n🔹 Phishing\\n🔹 Malware\\n🔹 Passwords\\n🔹 Firewalls\";";
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


        //------------------------------PrintTyping Method-------------------------------//
        private static void PrintTyping(string text, ConsoleColor color, int delay = 20)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay); // Delay for typing effect
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        //-----------------------------------------------------------------------------------//
    }

    }
//----------------------------------------------------------------------------------------------END OF PROGRAM-------------------------------------------------------------------------//
