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

using System;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    class Program
    {
        //---------------------------------MAIN METHOD -------------------------------//
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "🌸 CyberSecurity Chatbot 🌸";

            await AudioPlayer.PlayGreetingAudioAsync();
            ChatBotUI.DisplayGreeting();

            await Task.Delay(1000);
            string name = ChatBot.AskUsername();

            ChatBot.StartConversation(name);
        }
    }
}
//----------------------------------------------------------------------------------------------END OF PROGRAM-------------------------------------------------------------------------//
