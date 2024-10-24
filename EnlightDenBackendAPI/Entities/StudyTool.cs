using Org.BouncyCastle.Asn1.Mozilla;

namespace EnlightDenBackendAPI.Entities;

public class StudyTool
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ApplicationUser? User { get; set; }
    public required string UserId { get; set; }
    public List<Question>? Questions { get; set; }
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }
    public MindMap? MindMap { get; set; }
    public Guid MindMapId { get; set; }
    public Guid TopicId { get; set; }
    public ContentType ContentType { get; set; }
}

public enum ContentType
{
    Test = 1,
    FlashCardSet = 2,
}

public class GetStudyToolsDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string UserId { get; set; }
    public Guid ClassId { get; set; }
    public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
    public Guid MindMapId { get; set; }
    public ContentType ContentType { get; set; }
}

public class QuestionDTO
{
    public Guid Id { get; set; }
    public required string Request { get; set; }
    public required string Answer { get; set; }
    public QuestionType QuestionType { get; set; }
}

    public class GradeTestRequest
    {
        public Guid StudyToolId { get; set; }
        public List<UserAnswer> UserAnswers { get; set; }
    }

    public class UserAnswer
    {
        public Guid QuestionId { get; set; }
        public string UserResponse { get; set; }
    }
