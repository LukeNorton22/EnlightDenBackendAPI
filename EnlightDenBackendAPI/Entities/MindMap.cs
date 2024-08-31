namespace EnlightDenBackendAPI.Entities;

public class MindMap
{
    public Guid Id { get; set; }
    public List<MindMapTopic>? Topics { get; set; }
    public List<Guid> TopicIds { get; set; } = new List<Guid>();
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }

}
