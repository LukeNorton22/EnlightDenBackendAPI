using System.Text.Json.Serialization;
namespace EnlightDenBackendAPI.Entities

{
    public class StudyModule
    {
        public Guid Id { get; set; }
        public required Guid MindMapId { get; set; }    
        public Guid MindMapTopicId { get; set; }
        public required MindMapTopic MindMapTopic { get; set; }
        public required string MindMapTopicName { get; set; }
        public List<SubTopic> SubTopics { get; set; } = new List<SubTopic>();
        public Guid StudyToolId { get; set; }
        public required StudyTool StudyTool { get; set; }
    }

    public class SubTopic
    {
        public Guid Id  { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required Guid StudyModuleId { get; set; }
        public Guid MindMapTopicId { get; set; }

        [JsonIgnore]
        public StudyModule StudyModule { get; set; } = null!;
    }

    public class StudyModuleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MindMapId { get; set; }
        public Guid MindMapTopic { get; set; }
        public List<SubTopic> SubTopics { get; set; } = new List<SubTopic>();
    }

    public class StudyModuleSubTopicDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}



