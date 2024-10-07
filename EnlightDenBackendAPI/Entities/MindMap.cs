using Org.BouncyCastle.Security;

namespace EnlightDenBackendAPI.Entities;

public class MindMap
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MindMapTopic>? Topics { get; set; } = new List<MindMapTopic>();
    public List<Guid> TopicIds { get; set; } = new List<Guid>();
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }
    public ApplicationUser? User { get; set; }
    public required string UserId { get; set; }
    public Guid NoteId { get; set; }
}

public class GetMindMapDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MindMapTopicsDTO> Topics { get; set; } = new List<MindMapTopicsDTO>();
    public Guid NoteId { get; set; }
    public Guid ClassId { get; set; }
}

public class MindMapTopicsDTO
{
    public Guid Id { get; set; }
    public required string Topic { get; set; }
}

public class GenerateTestRequest
{
    public Guid ClassId { get; set; }
    public Guid MindMapId { get; set; }
    public Guid TopicId { get; set; }
    public string Name { get; set; }
    public Guid NoteId { get; set; }
}
