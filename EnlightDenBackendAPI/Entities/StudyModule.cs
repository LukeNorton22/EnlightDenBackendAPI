using System.Text.Json.Serialization;
namespace EnlightDenBackendAPI.Entities

{
    public class StudyModule
    {
        public Guid Id { get; set; }
        public required Guid MindMapId { get; set; }    
        public Guid MindMapTopicId { get; set; }
        public required MindMapTopic MindMapTopic { get; set; }
        public required string MindMapTopicName { get; set; } // main topic of Study Module
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

        public StudyModule StudyModule { get; set; } = null!;
    }

    public class StudyModuleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MindMapId { get; set; }
        public Guid MindMapTopicId { get; set; }
        public Guid StudyToolId { get; set; }
        public List<StudyModuleSubTopicDTO> SubTopics { get; set; } = new List<StudyModuleSubTopicDTO>();
    }

    public class StudyModuleSubTopicDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }

    public class GenerateStudyModuleRequestDto
    {
        public Guid MindMapId { get; set; }
        public Guid MindMapTopicId { get; set; }
        public string MindMapTopic { get; set; }
    }
}



