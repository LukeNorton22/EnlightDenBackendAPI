namespace EnlightDenBackendAPI.Entities;

public class Question
{
    public Guid Id { get; set; }
    public required string Request { get; set; }
    public required string Answer { get; set; }
    public QuestionType QuestionType { get; set; }
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }
    public StudyTool? StudyTool { get; set; }
    public Guid StudyToolId { get; set; }
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
}
