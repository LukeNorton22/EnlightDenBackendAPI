namespace EnlightDenBackendAPI.Entities

{
    public class StudyModule
    {
        public Guid Id { get; set; }
        public required string MainTopic { get; set; }
        public List<SubTopic> SubTopics { get; set; } = new List<SubTopic>();
        public List<PracticeTest> PracticeTests { get; set; } = new List<PracticeTest>();
        public Guid StudyToolId { get; set; }
        public required StudyTool StudyTool { get; set; }
        public required Guid MindMapId { get; set; }
    }

    public class SubTopic
    {
        public Guid Id  { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public Guid StudyModuleId { get; set; }
        public required StudyModule StudyModule { get; set; }
    }

    public class PracticeTest
    {
        public Guid Id  { get; set; }
        public List<PracticeQuestion> PracticeQuestions { get; set; } = new List<PracticeQuestion>();
        public Guid StudyModuleId  { get; set; }
        public required StudyModule StudyModule { get; set; }
    }

    public class PracticeQuestion
    {
        public Guid Id  { get; set; }
        public required string Request { get; set; }
        public required string Answer { get; set; }
        public PracticeQuestionType QuestionType { get; set; }
        public Guid PracticeTestId { get; set; }
        public required PracticeTest PracticeTest { get; set; }
    }

    public enum PracticeQuestionType
    {
        MultipleChoice  = 1,
        TrueFalse = 2,
        ShortAnswer = 3
    }
}



