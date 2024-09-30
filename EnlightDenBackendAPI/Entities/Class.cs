namespace EnlightDenBackendAPI.Entities;

public class Class
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ApplicationUser? User { get; set; } // Change from User to ApplicationUser
    public required string UserId { get; set; }
}

public class GetClassDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public required string UserId { get; set; }
}

public class CreateClassDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateClassDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
