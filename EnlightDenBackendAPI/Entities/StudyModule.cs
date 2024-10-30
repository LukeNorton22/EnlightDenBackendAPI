namespace EnlightDenBackendAPI.Entities
{
    public class StudyModule
    {
        public Guid Id { get; set; }
        public string MainTopic { get; set; }
        public List<SubTopic> SubTopics { get; set; } = new List<SubTopic>();
        public List<PracticeTest> PracticeTests { get; set; } = new List<PracticeTest>
        public Guid StudyToolId { get; set; }
        public StudyTool StudyTool { get; set; }
    }

    public class SubTopic
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class PracticeTest
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Guid StudyModuleId { get; set; }
    }
}
