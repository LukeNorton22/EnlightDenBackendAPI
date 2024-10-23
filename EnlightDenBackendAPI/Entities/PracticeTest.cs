namespace EnlightDenBackendAPI.Entities
{
    public class PracticeTest
    {
        public Guid Id { get; set; }
        public required Module Module { get; set; }
        public Guid ModuleId { get; set; }
        public required List<Question>? Questions { get; set; } = new List<Question>();
        public Guid QuestionId { get; set; }  
    }
}
