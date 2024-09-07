namespace EnlightDenBackendAPI.Entities;

public class MindMapTopic
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public MindMap? MindMap { get; set; }
    public Guid MindMapId { get; set; }
}
