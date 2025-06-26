using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    class UserProfile
    {
        //name, age, role, and interests of the user
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Role { get; set; } // e.g., "Student", "Professor"
        public List<string> Interests { get; set; } = new List<string>();

        // Constructor to initialize the user profile with a name and role
        public void AddInterest(string topic)
        {
            // Normalize the topic to lowercase to avoid duplicates
            if (!Interests.Contains(topic.ToLower()))
            {
                Interests.Add(topic.ToLower());
            }
        }
    }
}
//---------------------------------------------------------------------------------//