namespace EnlightDenBackendAPI.Entities;

public class Class
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; } 
    public User? User { get; set; }
    public Guid UserId { get; set; }
}
