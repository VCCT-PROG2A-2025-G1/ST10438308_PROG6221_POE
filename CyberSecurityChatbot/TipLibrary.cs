using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    public static class TipLibrary
    {
        public static readonly Dictionary<Topic, string[]> TipsByTopic = new Dictionary<Topic, string[]>
        {
            { Topic.Phishing, new[] {
                "Always check the sender's email address. Scammers often use slightly altered addresses.",
                "Hover over links before clicking to see the actual URL. If it looks suspicious, don't click!",
                "Be wary of urgent or threatening language. Phishing emails often try to create a sense of panic.",
                "Legitimate organizations will rarely ask for sensitive information like passwords or credit card numbers via email.",
                "If an email seems suspicious, contact the organization directly using a known phone number or official website, not through the email itself."
            }},
            { Topic.Passwords, new[] {
                "Use a unique password for every important account. Don't reuse passwords!",
                "Aim for a password that is at least 12-16 characters long, combining letters, numbers, and symbols.",
                "Consider using a passphrase – a series of unrelated words – which can be long and easy to remember.",
                "Enable two-factor authentication (2FA) whenever possible for an extra layer of security.",
                "Use a reputable password manager to securely store and generate complex passwords."
            }},

            //Malware Tips – Source: Norton Security (https://us.norton.com/blog/malware)
            { Topic.Malware, new[] {
                "Avoid downloading files or software from untrusted sources.",
                "Keep your operating system and antivirus software updated regularly.",
                "Do not click on suspicious pop-ups or ads offering free services or prizes.",
                "Use a reliable antivirus program and scan your device regularly.",
                "Be cautious with email attachments, especially from unknown senders."
         
            }},

            //2FA Tips – Source: Google Safety Center (https://safety.google/authentication/)
            { Topic.TwoFA, new[] {
                "Use an authenticator app like Google Authenticator or Authy for stronger protection.",
                "Avoid relying solely on SMS codes if possible; apps are more secure.",
                "Enable 2FA on all accounts that support it, especially email and financial apps.",
                "If given backup codes, store them in a safe place.",
                "Use biometric options like fingerprint or face ID for extra security where available."
            }},

            //VPN Tips – Source: Kaspersky (https://www.kaspersky.com/resource-center/definitions/what-is-a-vpn)
            { Topic.VPNs, new[] {
                "Use VPNs when connected to public Wi-Fi to prevent data snooping.",
                "Choose a trusted VPN provider with a no-logs policy.",
                "VPNs hide your IP and encrypt your internet traffic.",
                "Using a VPN can help you access content restricted to other regions.",
                "VPNs are essential for maintaining privacy on unsecured networks."
            }},

            //Privacy Tips – Source: StaySafeOnline.org (https://staysafeonline.org)
            { Topic.Privacy, new[] {
               "Limit the personal information you share online, especially on social media.",
               "Review and update your app permissions regularly.",
               "Disable location tracking when not in use.",
               "Use privacy-focused browsers and search engines when possible.",
               "Be mindful of oversharing personal details in public forums or posts."
            }},
        };
    }

}