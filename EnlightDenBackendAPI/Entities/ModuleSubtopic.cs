namespace EnlightDenBackendAPI.Entities
{
    public class ModuleTopic
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Module? Module { get; set; }
        public Guid ModuleId { get; set; }
    }
}
