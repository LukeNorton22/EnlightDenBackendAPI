using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EnlightDenBackendAPI.Entities; // Assuming your entities are in this namespace
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
    [Route("api/StudyTool")]
    public class StudyToolsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudyToolsController(
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

        [HttpPost("GenerateFlashcards")]
        public async Task<IActionResult> GenerateFlashcardsFromPdf(
            IFormFile file,
            Guid classId,
            Guid mindMapId,
            string name
        )
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

            var flashcards = await GenerateFlashcardsAsync(extractedText);

            await SaveStudyToolAndQuestions(
                flashcards,
                ContentType.FlashCardSet,
                user.Id,
                classId,
                mindMapId,
                name
            );

            return Ok(new { Flashcards = flashcards });
        }

        [HttpPost("GenerateTest")]
        public async Task<IActionResult> GenerateTestFromPdf(
            IFormFile file,
            Guid classId,
            Guid mindMapId,
            string name
        )
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

            var testQuestions = await GenerateTestQuestionsAsync(extractedText);

            await SaveStudyToolAndQuestions(
                testQuestions,
                ContentType.Test,
                user.Id,
                classId,
                mindMapId,
                name
            );

            return Ok(new { TestQuestions = testQuestions });
        }

        [HttpGet("GetAllStudyTools")]
        public async Task<ActionResult<List<GetStudyToolsDTO>>> GetAllStudyTools()
        {
            var result = await _context
                .StudyTools.Include(_ => _.Questions)
                .Select(st => new GetStudyToolsDTO
                {
                    Id = st.Id,
                    Name = st.Name,
                    UserId = st.UserId,
                    ClassId = st.ClassId,
                    MindMapId = st.MindMapId,
                    ContentType = st.ContentType,
                    Questions = st
                        .Questions.Select(q => new QuestionDTO
                        {
                            Id = q.Id,
                            Request = q.Request,
                            Answer = q.Answer,
                            QuestionType = q.QuestionType,
                        })
                        .ToList(),
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetStudyTool/{id}")]
        public async Task<ActionResult<GetStudyToolsDTO>> GetStudyToolById(Guid id)
        {
            var studyTool = await _context
                .StudyTools.Include(_ => _.Questions)
                .Where(st => st.Id == id)
                .Select(st => new GetStudyToolsDTO
                {
                    Id = st.Id,
                    Name = st.Name,
                    UserId = st.UserId,
                    ClassId = st.ClassId,
                    MindMapId = st.MindMapId,
                    ContentType = st.ContentType,
                    Questions = st
                        .Questions.Select(q => new QuestionDTO
                        {
                            Id = q.Id,
                            Request = q.Request,
                            Answer = q.Answer,
                            QuestionType = q.QuestionType,
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            if (studyTool == null)
            {
                return NotFound($"StudyTool with ID {id} not found.");
            }

            return Ok(studyTool);
        }

        [HttpDelete("DeleteStudyTool/{id}")]
        public async Task<IActionResult> DeleteStudyTool(Guid id)
        {
            var studyTool = await _context.StudyTools.FindAsync(id);

            if (studyTool == null)
            {
                return NotFound($"StudyTool with ID {id} not found.");
            }

            _context.StudyTools.Remove(studyTool);

            await _context.SaveChangesAsync();

            return Ok($"StudyTool with ID {id} has been deleted.");
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

        private async Task<List<string>> GenerateFlashcardsAsync(string text)
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
                                content = "You are a helpful assistant that generates educational flashcards.",
                            },
                            new
                            {
                                role = "user",
                                content = $"Create a set of at least 10 detailed flashcards based on the following text, label them Q: and A:: {text}",
                            },
                        },
                        max_tokens = 1500,
                        temperature = 0.7,
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

                var flashcards = SplitContentIntoPairs(choices, "Flashcard");

                return flashcards;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"OpenAI API error: {response.StatusCode} - {errorContent}"
                );
            }
        }

        private async Task<List<string>> GenerateTestQuestionsAsync(string text)
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
                                content = "You are a helpful assistant that generates educational test questions and answers from notes.",
                            },
                            new
                            {
                                role = "user",
                                content = $"Create a test consisting of at least 10 short-answer and true/false questions based on the following text, label them Q: and A:: {text}",
                            },
                        },
                        max_tokens = 1500,
                        temperature = 0.7,
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

                var testQuestions = SplitContentIntoPairs(choices, "Test Question");

                return testQuestions;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"OpenAI API error: {response.StatusCode} - {errorContent}"
                );
            }
        }

        private async Task SaveStudyToolAndQuestions(
            List<string> items,
            ContentType contentType,
            string userId,
            Guid classId,
            Guid mindMapId,
            string name
        )
        {
            var studyTool = new StudyTool
            {
                Id = Guid.NewGuid(),
                Name = name,
                UserId = userId,
                ClassId = classId,
                MindMapId = mindMapId,
                ContentType = contentType,
            };

            var questions = new List<Question>();

            foreach (var item in items)
            {
                var splitItem = item.Split(
                    new[] { "Q:", "A:" },
                    StringSplitOptions.RemoveEmptyEntries
                );
                if (splitItem.Length == 2)
                {
                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        Request = splitItem[0].Trim(),
                        Answer = splitItem[1].Trim(),
                        ClassId = classId,
                        QuestionType =
                            contentType == ContentType.Test
                                ? QuestionType.ShortAnswer
                                : QuestionType.MultipleChoice,
                        StudyToolId = studyTool.Id,
                    };

                    questions.Add(question);
                }
            }

            await _context.Questions.AddRangeAsync(questions);
            await _context.StudyTools.AddAsync(studyTool);

            await _context.SaveChangesAsync();
        }

        private List<string> SplitContentIntoPairs(string text, string itemType)
        {
            var items = new List<string>();
            var parts = text.Split(new[] { "Q: ", "A: " }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i += 2)
            {
                if (i + 1 < parts.Length)
                {
                    var question = parts[i].Trim();
                    var answer = parts[i + 1].Trim();
                    items.Add($"Q: {question} A: {answer}");
                }
            }

            return items;
        }
    }
}
