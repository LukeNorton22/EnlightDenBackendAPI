namespace EnlightDenBackendAPI.Entities
{
    public class Module
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<ModuleTopic>? Topics { get; set; } = new List<ModuleTopic>();
        public List<Guid> ModuleTopicIds { get; set; } = new List<Guid>();
        public Class? Class { get; set; }
        public Guid ClassId { get; set; }
        public ApplicationUser? User { get; set; }
        public required string UserId { get; set; }
        public Guid NoteId { get; set; }
    }
}
