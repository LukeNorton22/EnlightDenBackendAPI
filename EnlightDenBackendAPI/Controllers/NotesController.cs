using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration.UserSecrets;
using static ApplicationDbContext;

namespace EnlightDenBackendAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Notes")]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/notes
        [HttpGet]
        public IActionResult GetNotes()
        {
            var notes = _context
                .Notes.Select(note => new GetNoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    CreateDate = note.CreateDate,
                    UpdateDate = note.UpdateDate,
                    UserId = note.UserId,
                    ClassId = note.ClassId,
                    FilePath = note.FilePath,
                })
                .ToList();

            return Ok(notes);
        }

        [HttpGet("{id}")]
        public IActionResult GetNoteById(Guid id)
        {
            var NoteToGet = _context.Set<Note>().FirstOrDefault(note => note.Id == id);

            if (NoteToGet == null)
            {
                return BadRequest("Note not found.");
            }

            return Ok(NoteToGet);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNote([FromForm] CreateNoteDto createNoteDto)
        {
            // Ensure the provided UserId and ClassId exist
            var userIdClaim = User
                .Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier && Guid.TryParse(c.Value, out _)
                )
                ?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User is authenticated but no valid user ID claim found.");
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            var classEntity = await _context.Classes.FindAsync(createNoteDto.ClassId);

            if (user == null || classEntity == null)
            {
                return BadRequest("User or Class not found.");
            }

            // Handle file upload
            if (createNoteDto.File == null || createNoteDto.File.Length == 0)
            {
                return BadRequest("Please upload a valid file.");
            }

            var Uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(Uploads);

            var fileName = Path.GetFileName(createNoteDto.File.FileName);
            var filePath = Path.Combine(Uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createNoteDto.File.CopyToAsync(stream);
            }

            // Create a new Note instance using the DTO
            var note = new Note
            {
                Title = createNoteDto.Title,
                CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UserId = user.Id,
                ClassId = createNoteDto.ClassId,
                FilePath =
                    filePath // Store the file path
                ,
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var response = new GetNoteDto
            {
                Id = note.Id,
                Title = note.Title,
                CreateDate = note.CreateDate,
                UpdateDate = note.UpdateDate,
                UserId = note.UserId,
                ClassId = note.ClassId,
                FilePath = note.FilePath, // Return the file path
            };

            return CreatedAtAction(nameof(GetNotes), new { id = note.Id }, response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var NoteToDelete = _context.Set<Note>().FirstOrDefault(note => note.Id == id);

            if (NoteToDelete == null)
            {
                return BadRequest("Note not found.");
            }

            _context.Set<Note>().Remove(NoteToDelete);
            _context.SaveChanges();

            return Ok("Note deleted.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm] UpdateNoteDto updateDto, Guid id)
        {
            var noteToUpdate = await _context.Notes.FindAsync(id);

            if (noteToUpdate == null)
            {
                return BadRequest("Note not found");
            }

            noteToUpdate.Title = updateDto.Title;
            noteToUpdate.ClassId = updateDto.ClassId;
            noteToUpdate.UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // If a new file is provided, save it and update the path
            if (updateDto.File != null && updateDto.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(updateDto.File.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.File.CopyToAsync(stream);
                }

                noteToUpdate.FilePath = filePath; // Update the file path
            }

            await _context.SaveChangesAsync();

            var noteToReturn = new GetNoteDto
            {
                Id = noteToUpdate.Id,
                Title = noteToUpdate.Title,
                UserId = noteToUpdate.UserId,
                ClassId = noteToUpdate.ClassId,
                UpdateDate = noteToUpdate.UpdateDate,
                CreateDate = noteToUpdate.CreateDate,
                FilePath = noteToUpdate.FilePath, // Return the updated file path
            };

            return Ok(noteToReturn);
        }
    }
}
