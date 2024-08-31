namespace EnlightDenBackendAPI.Entities;

public class Note
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public long CreateDate { get; set; }
    public long UpdateDate { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }

}
