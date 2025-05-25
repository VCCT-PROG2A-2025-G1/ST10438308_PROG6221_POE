using System;
using System.Threading;
using System.Xml.Linq;

namespace CyberSecurityChatbot
{
    public static class ChatBotUI
    {
        //-----------------------------------DISPLAY GREETING METHOD-------------------------------------------//
        public static void DisplayGreeting()

        {
            // Set console properties for better visibility and aesthetics
            Console.OutputEncoding = System.Text.Encoding.UTF8; //For symbols
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
 ═════════════════════════════════════════════════════
           🌸 CYBERSECURITY AWARENESS BOT  🌸     
 ═════════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"******");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"/\_/\");
            Console.WriteLine(@"(o.o)");
            Console.WriteLine(@" >^<");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"******");

            // Print a welcome message with a typing effect
            PrintTyping("👋Hey there! \nWelcome to the Cybersecurity Awareness Bot. I’m here to help you stay safe online.", ConsoleColor.White, 50);
            Console.WriteLine(@"---------------------------------------------------------------------------------------------------------------------");
            Console.ResetColor();

            
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------//

        //-----------------------------------PRINT TYPING METHOD-------------------------------------------//
        public static void PrintTyping(string text, ConsoleColor color, int delay = 20)
        {
            // This method simulates a typing effect in the console
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//----------------------------------------------------------------------------------------------END OF CHATBOT UI-------------------------------------------------------------------------//