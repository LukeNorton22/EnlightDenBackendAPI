namespace EnlightDenBackendAPI.Entities;

public class StudySession
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

   
    public string ClassName {get; set;}

    
}

public class CreateStudySessionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
 
    public string  ClassName {get; set;}
}

public class GetStudySessionDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public int Day { get; set; }
    public int Month { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public required string UserId { get; set; }

    public string ClassName {get; set;}
}

public class UpdateStudySessionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }

    public string ClassName {get; set;}
}
