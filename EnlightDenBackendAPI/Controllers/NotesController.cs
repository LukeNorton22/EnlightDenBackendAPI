using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using EnlightDenBackendAPI.Entities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration.UserSecrets;
using Tesseract;
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

        [HttpGet("GetByClassId/{classId}")]
        public IActionResult GetNotesForUser(Guid classId)
        {
            var note = _context
                .Notes.Where(_ => _.ClassId == classId)
                .Select(n => new { n.ClassId })
                .ToList();

            var notes = _context
                .Notes.Where(_ => _.ClassId == classId)
                .Select(note => new GetNoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    CreateDate = note.CreateDate,
                    UpdateDate = note.UpdateDate,
                    UserId = note.UserId,
                    ClassId = note.ClassId,
                    FilePath = note.FilePath,
                    Content = note.Content,
                    HasMindMap = _context.MindMaps.Any(_ => _.NoteId == note.Id),
                })
                .ToList();

            return Ok(notes);
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

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploads);

            var fileName = Path.GetFileName(createNoteDto.File.FileName);
            var filePath = Path.Combine(uploads, fileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createNoteDto.File.CopyToAsync(stream);
            }

            // Extract text from PDF using iText 7 and Tesseract OCR
            string extractedText = ExtractTextFromPdf(filePath);

            // Create a new Note instance using the DTO and extracted text
            var note = new Note
            {
                Title = createNoteDto.Title,
                CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UserId = user.Id,
                ClassId = createNoteDto.ClassId,
                FilePath = filePath, // Store the file path
                Content = extractedText, // Store extracted text from PDF
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Prepare response
            var response = new GetNoteDto
            {
                Id = note.Id,
                Title = note.Title,
                CreateDate = note.CreateDate,
                UpdateDate = note.UpdateDate,
                UserId = note.UserId,
                ClassId = note.ClassId,
                FilePath = note.FilePath,
            };

            return CreatedAtAction(nameof(GetNotes), new { id = note.Id }, response);
        }

        // Method to extract text from PDF using iText 7 and Tesseract OCR
        // Method to extract text from PDF using iText 7 and Tesseract OCR
        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder textBuilder = new StringBuilder();
            string tessdataPath = @"./tessdata"; // Assuming tessdata is in the project root

            using (PdfReader pdfReader = new PdfReader(filePath))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);

                    // Extract embedded text using iText 7
                    var text = PdfTextExtractor.GetTextFromPage(page);

                    // If embedded text is found, append it
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        textBuilder.AppendLine(text);
                    }
                    else
                    {
                        // If no embedded text, try to extract images and perform OCR
                        var resources = page.GetResources();
                        var xObjectNames = resources.GetResourceNames();

                        foreach (var xObjectName in xObjectNames)
                        {
                            try
                            {
                                var xObject = resources.GetImage(xObjectName);
                                if (xObject != null)
                                {
                                    using (
                                        var imageStream = new MemoryStream(
                                            xObject.GetImageBytes(true)
                                        )
                                    )
                                    {
                                        using (
                                            var ocrEngine = new TesseractEngine(
                                                tessdataPath,
                                                "eng",
                                                EngineMode.Default
                                            )
                                        )
                                        {
                                            var imagePix = Pix.LoadFromMemory(
                                                imageStream.ToArray()
                                            );
                                            using (var pageText = ocrEngine.Process(imagePix))
                                            {
                                                textBuilder.AppendLine(pageText.GetText()); // Append OCR result
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log or handle exceptions related to image extraction or OCR
                                Console.WriteLine(
                                    $"Error processing image for OCR on page {i}: {ex.Message}"
                                );
                            }
                        }
                    }
                }
            }

            return textBuilder.ToString();
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
        public async Task<IActionResult> Update([FromBody] UpdateNoteDto updateDto, Guid id)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(
                    ", ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );
                return BadRequest($"Model state is invalid: {errors}");
            }

            var noteToUpdate = await _context.Notes.FindAsync(id);
            if (noteToUpdate == null)
            {
                return NotFound("Note not found");
            }

            noteToUpdate.Title = updateDto.Title;
            noteToUpdate.ClassId = updateDto.ClassId;
            noteToUpdate.UpdateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _context.SaveChangesAsync();

            var noteToReturn = new GetNoteDto
            {
                Id = noteToUpdate.Id,
                Title = noteToUpdate.Title,
                UserId = noteToUpdate.UserId,
                ClassId = noteToUpdate.ClassId,
                UpdateDate = noteToUpdate.UpdateDate,
                CreateDate = noteToUpdate.CreateDate,
                FilePath = noteToUpdate.FilePath,
            };

            return Ok(noteToReturn);
        }
    }
}
