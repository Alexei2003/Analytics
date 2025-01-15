using System.Diagnostics;

namespace Analytics.Server.Objects
{
    public class UserAnswers
    {
        public int Score { get; set; } = 0;

        public List<UserAnswer> Answers { get; set; } = new();

        public class UserAnswer() 
        { 
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}
