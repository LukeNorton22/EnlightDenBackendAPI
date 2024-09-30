namespace EnlightDenBackendAPI.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public long CreateDate { get; set; }
    public long UpdateDate { get; set; }
    public ApplicationUser? User { get; set; }
    public required string UserId { get; set; }
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }

    public required string FilePath { get; set; }
}

public class CreateNoteDto
{
    public required string Title { get; set; }
    public Guid ClassId { get; set; }
    public required IFormFile File { get; set; }
}

public class GetNoteDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public long CreateDate { get; set; }
    public long UpdateDate { get; set; }
    public required string UserId { get; set; }
    public Guid ClassId { get; set; }
    public required string FilePath { get; set; }
}

public class UpdateNoteDto
{
    public required string Title { get; set; }

    public long UpdateDate { get; set; }

    public Guid ClassId { get; set; }

    public IFormFile? File { get; set; }
}
