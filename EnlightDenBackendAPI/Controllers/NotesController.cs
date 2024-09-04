
using static ApplicationDbContext;
using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notes
[HttpGet]
public IActionResult GetNotes()
{
    
    
    var notes = _context.Notes
        .Select(note => new GetNoteDto
        {
            Id = note.Id,
            Title = note.Title,
            CreateDate = note.CreateDate,
            UpdateDate = note.UpdateDate,
            UserId = note.UserId,
            ClassId = note.ClassId
        })
        .ToList();

    

    return Ok(notes);
}

[HttpGet("{id}")]
public IActionResult GetNoteById(Guid id)
{
    
    
    var NoteToGet = _context.Set<Note>()
    .FirstOrDefault( note => note.Id == id);
    
     if (NoteToGet == null)
        {
            return BadRequest("Note not found.");
        }

    return Ok(NoteToGet);
}
[HttpPost("create")]
public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto createNoteDto)
{
    // Ensure the provided UserId and ClassId exist
    var user = await _context.Users.FindAsync(createNoteDto.UserId);
    var classEntity = await _context.Classes.FindAsync(createNoteDto.ClassId);

    if (user == null || classEntity == null)
    {
        return BadRequest("User or Class not found.");
    }

    // Create a new Note instance using the DTO
    var note = new Note
    {
        Title = createNoteDto.Title, // Ensure Title is set
        CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // Convert to Unix timestamp
        UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // Convert to Unix timestamp
        UserId = createNoteDto.UserId,
        ClassId = createNoteDto.ClassId
    };

    // No need to set the Id; it will be automatically generated
    _context.Notes.Add(note);
    await _context.SaveChangesAsync();

    var response = new GetNoteDto 
    {
        Id = note.Id,
        Title = note.Title,
        CreateDate = note.CreateDate,
        UpdateDate = note.UpdateDate,
        UserId = note.UserId,
        ClassId = note.ClassId
    };

    return CreatedAtAction(nameof(GetNotes), new { id = note.Id }, response);
}

[HttpDelete("{id}")]
public IActionResult Delete(Guid id)
{
    var NoteToDelete = _context.Set<Note>()
    .FirstOrDefault( note => note.Id == id);
    
     if (NoteToDelete == null)
        {
            return BadRequest("Note not found.");
        }

    _context.Set<Note>().Remove(NoteToDelete);
    _context.SaveChanges();
    

    return Ok("Note deleted.");

}

[HttpPut("{id}")]
public IActionResult Update([FromBody]UpdateNoteDto updateDto, Guid id)
{
    var NoteToUpdate = _context.Set<Note>()
    .FirstOrDefault(note => note.Id == id);

    if (NoteToUpdate == null)
    { 
        return BadRequest("Note not found");
    }

    NoteToUpdate.Title = updateDto.Title;
    NoteToUpdate.ClassId = updateDto.ClassId;

    _context.SaveChanges();

    var NoteToReturn = new GetNoteDto 
    {
        Id = NoteToUpdate.Id,
        Title = NoteToUpdate.Title,
        UserId = NoteToUpdate.UserId,
        ClassId = NoteToUpdate.ClassId,
        UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        CreateDate = NoteToUpdate.CreateDate

    };

    return Ok(NoteToReturn);

}



    }
}