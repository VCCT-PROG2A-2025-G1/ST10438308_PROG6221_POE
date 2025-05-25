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
    //adding an enumeration for the topics 
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

    // This class generates responses based on user input
    public static class ResponseGenerator
    {
        private static Random random = new Random(); // Initialize Random once

        // Array for phishing tips
        private static string[] phishingTips = new string[]
        {
            "Always check the sender's email address. Scammers often use slightly altered addresses.",
            "Hover over links before clicking to see the actual URL. If it looks suspicious, don't click!",
            "Be wary of urgent or threatening language. Phishing emails often try to create a sense of panic.",
            "Legitimate organizations will rarely ask for sensitive information like passwords or credit card numbers via email.",
            "If an email seems suspicious, contact the organization directly using a known phone number or official website, not through the email itself."
        };



        // Array for password tips
        private static string[] passwordTips = new string[]
        {
            "Use a unique password for every important account. Don't reuse passwords!",
            "Aim for a password that is at least 12-16 characters long, combining letters, numbers, and symbols.",
            "Consider using a passphrase – a series of unrelated words – which can be long and easy to remember.",
            "Enable two-factor authentication (2FA) whenever possible for an extra layer of security.",
            "Use a reputable password manager to securely store and generate complex passwords."
        };

        // 🛡️ Malware Tips – Source: Norton Security (https://us.norton.com/blog/malware)
        private static string[] malwareTips = new string[]
        {
        "Avoid downloading files or software from untrusted sources.",
        "Keep your operating system and antivirus software updated regularly.",
        "Do not click on suspicious pop-ups or ads offering free services or prizes.",
        "Use a reliable antivirus program and scan your device regularly.",
        "Be cautious with email attachments, especially from unknown senders."
        };

        // 🔐 2FA Tips – Source: Google Safety Center (https://safety.google/authentication/)
        private static string[] twoFATips = new string[]
        {
        "Use an authenticator app like Google Authenticator or Authy for stronger protection.",
        "Avoid relying solely on SMS codes if possible; apps are more secure.",
        "Enable 2FA on all accounts that support it, especially email and financial apps.",
        "If given backup codes, store them in a safe place.",
        "Use biometric options like fingerprint or face ID for extra security where available."
        };

        // 🌐 VPN Tips – Source: Kaspersky (https://www.kaspersky.com/resource-center/definitions/what-is-a-vpn)
        private static string[] vpnTips = new string[]
        {
        "Use VPNs when connected to public Wi-Fi to prevent data snooping.",
        "Choose a trusted VPN provider with a no-logs policy.",
        "VPNs hide your IP and encrypt your internet traffic.",
        "Using a VPN can help you access content restricted to other regions.",
        "VPNs are essential for maintaining privacy on unsecured networks."
        };

        // 🕵️ Privacy Tips – Source: StaySafeOnline.org (https://staysafeonline.org)
        private static string[] privacyTips = new string[]
        {
        "Limit the personal information you share online, especially on social media.",
        "Review and update your app permissions regularly.",
        "Disable location tracking when not in use.",
        "Use privacy-focused browsers and search engines when possible.",
        "Be mindful of oversharing personal details in public forums or posts."
        };

        public static (string response, Topic topic) GetResponseWithTopic(string input, Topic currentTopic, string name)
        {
            input = input.ToLower();

            if (input.Contains("more") || input.Contains("details") || input.Contains("explain"))
            {
                return currentTopic switch
                {
                    Topic.Phishing => (phishingTips[random.Next(phishingTips.Length)], Topic.Phishing),
                    Topic.Passwords => (passwordTips[random.Next(passwordTips.Length)], Topic.Passwords),
                    Topic.Malware => (malwareTips[random.Next(malwareTips.Length)], Topic.Malware),
                    Topic.Encryption => ("There are two main types of encryption: symmetric and asymmetric. It's the foundation of secure communication.", Topic.Encryption),
                    Topic.TwoFA => (twoFATips[random.Next(twoFATips.Length)], Topic.TwoFA),
                    Topic.VPNs => (vpnTips[random.Next(vpnTips.Length)], Topic.VPNs),
                    Topic.Privacy => (privacyTips[random.Next(privacyTips.Length)], Topic.Privacy),
                    _ => ("Could you tell me what you'd like more details about?", Topic.None)
                };
            }

            // New tip triggers
            if (input.Contains("phishing tip") || input.Contains("phishing advice"))
                return (phishingTips[random.Next(phishingTips.Length)], Topic.Phishing);

            if (input.Contains("password tip") || input.Contains("password advice"))
                return (passwordTips[random.Next(passwordTips.Length)], Topic.Passwords);

            if (input.Contains("malware tip") || input.Contains("malware advice"))
                return (malwareTips[random.Next(malwareTips.Length)], Topic.Malware);

            if (input.Contains("2fa tip") || input.Contains("two-factor tip"))
                return (twoFATips[random.Next(twoFATips.Length)], Topic.TwoFA);

            if (input.Contains("vpn tip") || input.Contains("vpn advice"))
                return (vpnTips[random.Next(vpnTips.Length)], Topic.VPNs);

            if (input.Contains("privacy tip") || input.Contains("privacy advice"))
                return (privacyTips[random.Next(privacyTips.Length)], Topic.Privacy);

            //Keyword Triggers

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
    }
}