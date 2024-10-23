namespace EnlightDenBackendAPI.Entities
{
    public class Module
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<ModuleSubtopic>? Subtopics { get; set; } = new List<ModuleSubtopic>();
        public List<Guid> SubtopicIds { get; set; } = new List<Guid>();
        public Class? Class { get; set; }
        public Guid ClassId { get; set; }
        public ApplicationUser? User { get; set; }
        public required string UserId { get; set; }
        public Note? Note { get; set; }
        public Guid NoteId { get; set; }
    }


}
