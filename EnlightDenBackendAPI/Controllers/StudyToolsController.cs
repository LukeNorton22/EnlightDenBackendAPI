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

        [HttpPost("GenerateTestFromTopic")]
        public async Task<IActionResult> GenerateTestFromTopic(
            [FromBody] GenerateTestRequest request
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

            var mindMap = await _context.MindMaps.FindAsync(request.MindMapId);
            // Fetch the note content using the noteId from the request
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == mindMap.NoteId);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            string noteContent = note.Content;

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(noteContent))
            {
                return BadRequest("Name and note content are required.");
            }

            var topicName = await _context.MindMapTopics.FindAsync(request.TopicId);

            // Generate the test questions based on the note content and the topic
            var testQuestions = await GenerateTestQuestionsTopicAsync(noteContent, topicName.Name);

            // Save the generated test questions and study tool
            var generatedTestId = await SaveStudyToolAndQuestionsTopic(
                testQuestions,
                ContentType.Test,
                user.Id,
                mindMap.ClassId,
                request.MindMapId,
                request.Name,
                request.TopicId
            );

            // Return the generated test ID (adjust this based on your actual logic)
            return Ok(new { TestId = generatedTestId });
        }

        [HttpGet("CheckExistingTest/{topicId}")]
        public async Task<IActionResult> CheckExistingTest(Guid topicId)
        {
            var testExists = await _context.StudyTools.FirstOrDefaultAsync(t =>
                t.TopicId == topicId && t.ContentType == ContentType.Test
            );

            var testBool = await _context.StudyTools.AnyAsync(t =>
                t.TopicId == topicId && t.ContentType == ContentType.Test
            );

            return Ok(new { TestExists = testBool, TestId = testExists?.Id }); // Return as an object with a key
        }

        [HttpGet("CheckExistingFlashcard/{topicId}")]
        public async Task<IActionResult> CheckExistingFlashCard(Guid topicId)
        {
            var testExists = await _context.StudyTools.FirstOrDefaultAsync(t =>
                t.TopicId == topicId && t.ContentType == ContentType.FlashCardSet
            );

            var testBool = await _context.StudyTools.AnyAsync(t =>
                t.TopicId == topicId && t.ContentType == ContentType.FlashCardSet
            );

            return Ok(new { FlashCardExists = testBool, FlashCardId = testExists?.Id }); // Return as an object with a key
        }

        private async Task<List<string>> GenerateTestQuestionsTopicAsync(string text, string topic)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            )
            {
                Content = JsonContent.Create(
                    new
                    {
                        model = "gpt-4", // Using GPT-4 for accuracy and detail
                        messages = new[]
                        {
                            new
                            {
                                role = "system",
                                content = @"
You are an expert educational assistant tasked with generating highly specific test questions.
Your job is to create test questions that are closely tied to the provided topic. 
The output must strictly follow the given format, and irrelevant or off-topic content should be excluded entirely.",
                            },
                            new
                            {
                                role = "user",
                                content = $@"
The topic for these test questions is: **'{topic}'**.
Use the most relevant content from the following notes to create **a set of test questions**.
Each question must relate directly to the topic.

Here are the **strict instructions**:
1. Only create questions that align **directly with the topic** '{topic}'.
2. Ignore any content that does not fit the topic precisely.
3. Ensure a mix of **short-answer** and **true/false** questions.
4. Format the output like this:

   Q: [Your question here]  
   A: [The correct answer extracted from the notes]

5. Do not include any text outside of the **Q: and A:** format.  
6. Every answer must be concise, accurate, and directly drawn from the provided notes.

Below are the notes you should use:

{text}
",
                            },
                        },
                        max_tokens = 4096,
                        temperature = 0.2, // Low temperature for precision
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

                // Split the generated content into Q: and A: pairs
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

        private async Task<Guid> SaveStudyToolAndQuestionsTopic(
            List<string> items,
            ContentType contentType,
            string userId,
            Guid classId,
            Guid mindMapId,
            string name,
            Guid topicId
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
                TopicId = topicId,
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

            return studyTool.Id;
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

        [HttpPost("GenerateFlashcardsFromTopic")]
        public async Task<IActionResult> GenerateFlashcardsFromTopic(
            [FromBody] GenerateTestRequest request
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

            var mindMap = await _context.MindMaps.FindAsync(request.MindMapId);
            // Fetch the note content using the noteId from the request
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == mindMap.NoteId);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            string noteContent = note.Content;

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(noteContent))
            {
                return BadRequest("Name and note content are required.");
            }

            var topicName = await _context.MindMapTopics.FindAsync(request.TopicId);

            // Generate the flashcards based on the note content and the topic
            var flashcards = await GenerateFlashcardsTopicAsync(noteContent, topicName.Name);

            // Save the generated flashcards and study tool
            var generatedFlashcardsId = await SaveStudyToolAndQuestionsTopic(
                flashcards,
                ContentType.FlashCardSet,
                user.Id,
                mindMap.ClassId,
                request.MindMapId,
                request.Name,
                request.TopicId
            );

            // Return the generated flashcards ID (adjust this based on your actual logic)
            return Ok(new { FlashcardsId = generatedFlashcardsId });
        }

        private async Task<List<string>> GenerateFlashcardsTopicAsync(string text, string topic)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            )
            {
                Content = JsonContent.Create(
                    new
                    {
                        model = "gpt-4", // Using GPT-4 for precision
                        messages = new[]
                        {
                            new
                            {
                                role = "system",
                                content = @"
You are an expert educational assistant focused on generating highly precise flashcards. 
Your sole task is to create flashcards that align strictly with the given topic. 
Any content or notes that are not directly relevant to the topic should be ignored, no matter how useful they may seem.
Ensure that each flashcard’s answer contains concise, topic-specific knowledge.",
                            },
                            new
                            {
                                role = "user",
                                content = $@"
You are provided with the following topic: '{topic}'.
Your task is to create a set of **hyper-specific flashcards** based exclusively on the most relevant parts of the following notes:

{text}

**Strict Instructions:** 
1. Do NOT generate any content unrelated to the topic '{topic}'.
2. Ignore any tangential information from the notes.
3. Ensure each flashcard has a **clear question or prompt** and a **precise answer**, extracted ONLY from the text.

Format each flashcard like this:
Q: [Highly specific question related to the topic]
A: [Accurate answer from the notes]",
                            },
                        },
                        max_tokens = 4096,
                        temperature = 0.2, // Lower temperature for even more factual precision
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
    }
}
