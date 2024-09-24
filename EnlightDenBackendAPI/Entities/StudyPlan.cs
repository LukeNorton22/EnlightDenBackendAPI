namespace EnlightDenBackendAPI.Entities;

public class StudyPlan
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public int Day { get; set; }
    public int Month { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
