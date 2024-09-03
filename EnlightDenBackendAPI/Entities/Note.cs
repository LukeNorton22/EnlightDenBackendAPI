namespace EnlightDenBackendAPI.Entities;

public class Note
{
     public Guid Id { get; set; } = Guid.NewGuid(); 
    public required string Title { get; set; }
    public long CreateDate { get; set; }
    public long UpdateDate { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public Class? Class { get; set; }
    public Guid ClassId { get; set; }

}

public class CreateNoteDto {
    public required string Title {get; set; }
    public Guid UserId  {get; set; }
    public Guid ClassId  {get; set; }
}

public class GetNoteDto {

    public Guid Id {get; set;}
    public  required string Title {get; set;}
    public long CreateDate { get; set; }
    public long UpdateDate { get; set; }
    public Guid UserId {get; set;}
    public Guid ClassId { get; set;}


}

public class UpdateNoteDto {

    
    public  required string Title {get; set;}
   
    public long UpdateDate { get; set; }
    
    public Guid ClassId { get; set; }
}