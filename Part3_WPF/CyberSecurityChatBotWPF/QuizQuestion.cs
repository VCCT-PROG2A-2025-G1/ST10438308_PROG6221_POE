using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityChatBotWPF
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; } = new();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; } = false; // for True/False questions
    }
}
