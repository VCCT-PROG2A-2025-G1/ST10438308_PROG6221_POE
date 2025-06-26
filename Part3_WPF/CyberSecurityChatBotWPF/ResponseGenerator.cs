//Funi Mapunda
//ST10438308
//BCAD2 GR1

//References: 
//codecademy.com
//stackoverflow.com
//chatgpt.com

using System;
using System.Xml.Linq;
using System.Threading; //for type writer effect??
using CyberSecurityChatbot;


namespace CyberSecurityChatbot
{
    //-----------------------------------TOPIC ENUMERATION -------------------------------//
    // This enum defines the different topics the chatbot can discuss
    public enum Topic
    {
        None,
        Phishing,
        Malware,
        Passwords,
        Firewalls,
        Scams,
        Privacy,
        Encryption,
        TwoFA,
        VPNs,

    }
    //------------------------------------------------------------------//

    //-----------------------------------RESPONSE GENERATOR CLASS -------------------------------//
    // This class generates responses based on user input
    public static class ResponseGenerator
    {
        // Random instance for selecting tips
        private static Random random = new Random();


        //-----------------------------------GET RESPONSE WITH TOPIC METHOD -------------------------------//
        // This method generates a response based on the user's input and current topic
        public static (string response, Topic topic) GetResponseWithTopic(string input, Topic currentTopic, string name)
        {
            input = input.ToLower(); // Normalize input to lowercase for easier matching

            // Check if user wants more details or explanations, then get a tip from TipLibrary if available
            if (input.Contains("more") || input.Contains("details") || input.Contains("explain"))
            {
                if (TipLibrary.TipsByTopic.TryGetValue(currentTopic, out var tips))
                {
                    string tip = tips[random.Next(tips.Length)];
                    return (tip, currentTopic);
                }
                else if (currentTopic == Topic.Encryption)
                {
                    return ("There are two main types of encryption: symmetric and asymmetric. It's the foundation of secure communication.", Topic.Encryption);
                }
                else
                {
                    return ("Could you tell me what you'd like more details about?", Topic.None);
                }
            }

            // Specific tip requests
            if (input.Contains("phishing tip") || input.Contains("phishing advice"))
                return GetRandomTip(Topic.Phishing);

            if (input.Contains("password tip") || input.Contains("password advice"))
                return GetRandomTip(Topic.Passwords);

            if (input.Contains("malware tip") || input.Contains("malware advice"))
                return GetRandomTip(Topic.Malware);

            if (input.Contains("2fa tip") || input.Contains("two-factor tip"))
                return GetRandomTip(Topic.TwoFA);

            if (input.Contains("vpn tip") || input.Contains("vpn advice"))
                return GetRandomTip(Topic.VPNs);

            if (input.Contains("privacy tip") || input.Contains("privacy advice"))
                return GetRandomTip(Topic.Privacy);

            // Keyword triggers for general explanations
            if (input.Contains("phishing"))
                return ("Phishing is when attackers try to trick you into sharing sensitive info using fake emails or links.", Topic.Phishing);

            if (input.Contains("malware"))
                return ("Malware is software designed to damage your system. Use antivirus and be cautious of suspicious files.", Topic.Malware);

            if (input.Contains("password"))
                return ("Use complex passwords with upper/lowercase letters, symbols, and numbers. Avoid using personal details.", Topic.Passwords);

            if (input.Contains("firewall"))
                return ("A firewall monitors traffic to block dangerous connections. It's your first line of digital defense.", Topic.Firewalls);

            if (input.Contains("scam"))
                return ("Online scams try to trick you into giving away money or sensitive information. Always verify links and avoid unknown contacts.", Topic.Scams);

            if (input.Contains("privacy"))
                return ("Protect your privacy by limiting the personal information you share online and adjusting your social media settings.", Topic.Privacy);

            if (input.Contains("encryption"))
                return ("Encryption scrambles your data so only authorized users can access it. It’s essential for protecting sensitive information.", Topic.Encryption);

            if (input.Contains("2fa") || input.Contains("two-factor"))
                return ("Two-factor authentication adds an extra layer of security by requiring a code in addition to your password.", Topic.TwoFA);

            if (input.Contains("vpn"))
                return ("A VPN hides your IP and encrypts your internet traffic, making your browsing more private and secure.", Topic.VPNs);

            if (input.Contains("how are you"))
                return ($"I'm great, {name}! Always ready to fight cyber threats.", Topic.None);

            if (input.Contains("what can i ask"))
                return ("You can ask anything about phishing, malware, passwords, firewalls, scams, privacy, encryption, 2FA, VPNs, and tips on staying safe online!", Topic.None);

            if (input.Contains("how do you work"))
                return ("I use simple keyword recognition to give you answers. I'm not AI, but I do try my best!", Topic.None);

            return ($"Sorry {name}, I don’t understand that yet. Try asking about:\n🔹 Phishing\n🔹 Malware\n🔹 Passwords\n🔹 Firewalls\n🔹 Scams\n🔹 Privacy\n🔹 Encryption\n🔹 2FA\n🔹 VPNs\n🔹 Phishing tips\n🔹 Password tips", Topic.None);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        //-----------------------------------GET RANDOM TIP METHOD -------------------------------//
        // This method retrieves a random tip from the TipLibrary based on the specified topic
        private static (string response, Topic topic) GetRandomTip(Topic topic)
        {
            if (TipLibrary.TipsByTopic.TryGetValue(topic, out var tips)) 
            {
                string tip = tips[random.Next(tips.Length)];
                return (tip, topic);
            }
            return ("Sorry, I don't have tips on that topic yet.", Topic.None);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//------------------------------------------------------------------------------------------------END OF RESPONSE GENERATOR CLASS-------------------------------------------------------------------------//
