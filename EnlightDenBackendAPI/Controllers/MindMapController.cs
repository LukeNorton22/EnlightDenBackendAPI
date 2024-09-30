using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EnlightDenBackendAPI.Entities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace EnlightDenBackendAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/MindMap")]
    public class MindMapController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MindMapController(
            IConfiguration configuration,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _httpClient = new HttpClient();
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("CreateMindMap")]
        public async Task<IActionResult> UploadPdfAndGenerateMindMap(IFormFile file, Guid classId)
        {
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

            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid PDF file.");
            }

            string extractedText;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                extractedText = ExtractTextFromPdf(stream);
            }

            var mindMapTopics = await GenerateMindMapTopicsAsync(extractedText);

            var mindMap = new MindMap
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                ClassId = classId,
                UserId = user.Id,
                Topics = new List<MindMapTopic>(),
            };

            foreach (var topic in mindMapTopics)
            {
                var mindMapTopic = new MindMapTopic
                {
                    Id = Guid.NewGuid(),
                    Name = topic,
                    MindMap = mindMap,
                    MindMapId = mindMap.Id,
                };

                mindMap.Topics.Add(mindMapTopic);
                mindMap.TopicIds.Add(mindMapTopic.Id);
            }

            _context.MindMaps.Add(mindMap);
            await _context.SaveChangesAsync();

            return Ok(new { MindMapId = mindMap.Id, MindMapTopics = mindMapTopics });
        }

        [HttpPost("CreateMindMapFromNote/{noteId}")]
        public async Task<IActionResult> CreateMindMapFromNote(Guid noteId)
        {
            // Retrieve the note from the database
            var note = await _context.Notes.FindAsync(noteId);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            // Check if the file exists
            if (!System.IO.File.Exists(note.FilePath))
            {
                return NotFound("File not found.");
            }

            string extractedText;
            using (var stream = new FileStream(note.FilePath, FileMode.Open, FileAccess.Read))
            {
                extractedText = ExtractTextFromPdf(stream); // Implement this method to extract text from PDF
            }

            // Generate mind map topics from the extracted text
            var mindMapTopics = await GenerateMindMapTopicsAsync(extractedText);

            var mindMap = new MindMap
            {
                Id = Guid.NewGuid(),
                Name = note.Title, // You can customize this name
                ClassId = note.ClassId,
                UserId = note.UserId,
                Topics = new List<MindMapTopic>(),
            };

            foreach (var topic in mindMapTopics)
            {
                var mindMapTopic = new MindMapTopic
                {
                    Id = Guid.NewGuid(),
                    Name = topic,
                    MindMap = mindMap,
                    MindMapId = mindMap.Id,
                };

                mindMap.Topics.Add(mindMapTopic);
                mindMap.TopicIds.Add(mindMapTopic.Id);
            }

            _context.MindMaps.Add(mindMap);
            await _context.SaveChangesAsync();

            return Ok(new { MindMapId = mindMap.Id, MindMapTopics = mindMapTopics });
        }

        [HttpGet("GetAllMindMaps")]
        public async Task<ActionResult<List<GetMindMapDTO>>> GetAllMindMaps()
        {
            var mindMaps = await _context
                .MindMaps.Include(mm => mm.Topics)
                .Select(mm => new GetMindMapDTO
                {
                    Id = mm.Id,
                    Name = mm.Name,
                    Topics = mm
                        .Topics.Select(t => new MindMapTopicsDTO { Topic = t.Name })
                        .ToList(),
                })
                .ToListAsync();

            return Ok(mindMaps);
        }

        [HttpGet("GetMindMapById/{id}")]
        public async Task<ActionResult<GetMindMapDTO>> GetMindMapById(Guid id)
        {
            var mindMap = await _context
                .MindMaps.Include(mm => mm.Topics)
                .Where(mm => mm.Id == id)
                .Select(mm => new GetMindMapDTO
                {
                    Id = mm.Id,
                    Name = mm.Name,
                    Topics = mm
                        .Topics.Select(t => new MindMapTopicsDTO { Topic = t.Name })
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            if (mindMap == null)
            {
                return NotFound($"MindMap with ID {id} not found.");
            }

            return Ok(mindMap);
        }

        [HttpDelete("DeleteMindMap/{id}")]
        public async Task<IActionResult> DeleteMindMap(Guid id)
        {
            var mindMap = await _context
                .MindMaps.Include(mm => mm.Topics)
                .FirstOrDefaultAsync(mm => mm.Id == id);

            if (mindMap == null)
            {
                return NotFound($"MindMap with ID {id} not found.");
            }

            _context.MindMaps.Remove(mindMap);
            await _context.SaveChangesAsync();

            return Ok($"MindMap with ID {id} has been deleted.");
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder extractedText = new StringBuilder();

            using (var pdfReader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    var pageText = PdfTextExtractor.GetTextFromPage(
                        pdfDocument.GetPage(page),
                        strategy
                    );
                    extractedText.Append(pageText);
                }
            }

            return extractedText.ToString();
        }

        private async Task<List<string>> GenerateMindMapTopicsAsync(string text)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            )
            {
                Content = JsonContent.Create(
                    new
                    {
                        model = "gpt-3.5-turbo",
                        messages = new[]
                        {
                            new
                            {
                                role = "system",
                                content = "You are a helpful assistant that generates mind map topics.",
                            },
                            new
                            {
                                role = "user",
                                content = $"Extract the main topics from the following text, only return the topic names from this text, and do not number them: {text}",
                            },
                        },
                        max_tokens = 1500,
                        temperature = 0.5,
                    }
                ),
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(resultContent);
                var choices = jsonResponse["choices"]?.First?["message"]?["content"]?.ToString();
                var topics = choices
                    ?.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

                return topics ?? new List<string>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"OpenAI API error: {response.StatusCode} - {errorContent}"
                );
            }
        }
    }
}
