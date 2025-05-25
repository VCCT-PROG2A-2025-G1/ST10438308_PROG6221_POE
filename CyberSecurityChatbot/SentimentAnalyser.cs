using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    class SentimentAnalyser
    {
        //-----------------------------------DETETCT SENTIMENT METHOD-------------------------------------------//
        public static string DetectSentiment(String input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("nervous"))
                return "worried";
            if (input.Contains("frustrated") || input.Contains("confused") || input.Contains("annoyed"))
                return "frustrated";
            if (input.Contains("curious") || input.Contains("interested") || input.Contains("wondering"))
                return "curious";

            return "";
        }
        //---------------------------------------------------------------------------------------//

        //-----------------------------------GET SENTIMENT RESPONSE METHOD-------------------------------------------//
        public static string GetSentimentResponse(string sentiment)
        {
            return sentiment switch
            {
                "worried" => "It's completely normal to feel worried about cybersecurity. Let's address your concerns together.",
                "frustrated" => "I understand that cybersecurity can be frustrating. I'm here to help clarify things for you.",
                "curious" => "Curiosity is a great starting point! What would you like to know more about?",
                _ => ""
            };
        }
        //---------------------------------------------------------------------------------------//
    }
}
//----------------------------------------------------------------------------------------------END OF SENTIMENT ANALYSER-------------------------------------------------------------------------//
