namespace EnlightDenBackendAPI.Entities;

public class StudyTool
{
    public Guid Id { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public List<Question>? Questions { get; set; }
    public List<Guid> QuestionIds { get; set; } = new List<Guid>();
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }
    public MindMapTopic? MindMapTopic { get; set; }
    public Guid MindMapTopicId { get; set; }
    public ContentType ContentType { get; set; }
}

public enum ContentType
{
    Test = 1,
    FlashCardSet = 2, 
}
