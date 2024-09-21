namespace EnlightDenBackendAPI.Entities;

public class Class
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
}

public class GetClassDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
}

public class CreateClassDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}

public class UpdateClassDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
