namespace Analytics.Server.Objects
{
    public class Survey
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PhotoUrl { get; set; }

        public List<Question> Questions { get; set; } = new();
        public DateTime CreationData { get; set; }
        public DateTime EndData { get; set; }

        public class Question()
        {
            public int Id { get; set; }

            public string Text { get; set; }
            public List<Answer> Answers { get; set; } = new();
            public class Answer()
            {
                public int Id { get; set; }
                public string Text { get; set; }
                public int Score { get; set; }
            }
        }
    }
}
