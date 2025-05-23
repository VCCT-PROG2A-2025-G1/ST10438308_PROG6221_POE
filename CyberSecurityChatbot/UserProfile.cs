using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    class UserProfile
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Role { get; set; } // e.g., "Student", "Professor"
        public List<string> Interests { get; set; } = new List<string>();

        public void AddInterest(string topic)
        {
            if(!Interests.Contains(topic.ToLower()))
            {
                Interests.Add(topic.ToLower());
            }
        }
    }
}
