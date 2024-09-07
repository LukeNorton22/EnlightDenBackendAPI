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
    public User? User { get; set; }
    public Guid UserId { get; set; }

}

public class GetMindMapDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MindMapTopicsDTO> Topics { get; set; } = new List<MindMapTopicsDTO>();

}

public class MindMapTopicsDTO
{
    public required string Topic { get; set; }
}
