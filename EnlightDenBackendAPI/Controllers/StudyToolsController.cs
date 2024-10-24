﻿using System.Collections.Generic;
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
                t.TopicId == topicId
            );

            var testBool = await _context.StudyTools.AnyAsync(t => t.TopicId == topicId);

            return Ok(new { TestExists = testBool, TestId = testExists?.Id }); // Return as an object with a key
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
                        model = "gpt-4", // Using GPT-4 for more accurate and detailed responses
                        messages = new[]
                        {
                            new
                            {
                                role = "system",
                                content = "You are an expert educational assistant tasked with generating highly accurate test questions based on detailed notes. Your answers must be directly extracted from the given notes.",
                            },
                            new
                            {
                                role = "user",
                                content = $@"
                        You are provided with the following topic: '{topic}'.
                        Your task is to create a set of **at least 10 detailed test questions**. Each question must be closely related to this topic, and the answer must be **directly and exclusively derived** from the following notes:

                        {text}

                        The questions should be **true/false** types. Ensure that all questions are related to the topic '{topic}' and that no irrelevant questions are generated.

                        Format each question and answer like this:
                        Q: [Your question here]
                        A: [True/False]",
                            },
                        },
                        max_tokens = 4096,
                        temperature = 0.3, // Reduced temperature for more factual and precise responses
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

        // Generate test questions based on the provided text

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
                                content = $"Create a test consisting of at least 10 true/false questions based on the following text, label them Q: and A:: {text}",
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

        [HttpPost("GradeTest")]
        public async Task<IActionResult> GradeTest([FromBody] GradeTestRequest request)
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

            var studyTool = await _context.StudyTools.Include(st => st.Questions)
                .FirstOrDefaultAsync(st => st.Id == request.StudyToolId);

            if (studyTool == null)
            {
                return NotFound("Study tool not found.");
            }

            var gradingResults = new List<object>();
            int correctCount = 0;

            foreach (var userAnswer in request.UserAnswers)
            {
                var correctAnswer = studyTool.Questions
                    .FirstOrDefault(q => q.Id == userAnswer.QuestionId)?.Answer.Trim().ToLower();

                if (correctAnswer != null)
                {
                    var isCorrect = await IsAnswerCorrect(userAnswer.UserResponse, correctAnswer);
                    gradingResults.Add(new { QuestionId = userAnswer.QuestionId, Correct = isCorrect ? 1 : 0 });

                    if (isCorrect)
                    {
                        correctCount++;
                    }
                }
            }

            var score = correctCount;

            return Ok(new { Score = score, GradingResults = gradingResults });
        }

        private async Task<bool> IsAnswerCorrect(string userResponse, string correctAnswer)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            )
            {
                Content = JsonContent.Create(
                    new
                    {
                        model = "gpt-4",
                        messages = new[]
                        {
                            new
                            {
                                role = "system",
                                content = "You are an expert educational assistant tasked with grading test answers. Determine if the user's answer is close enough to the correct answer."
                            },
                            new
                            {
                                role = "user",
                                content = $@"
                                Correct Answer: '{correctAnswer}'
                                User Response: '{userResponse}'
                                Is the user's response close enough to the correct answer? Respond with 'yes' or 'no'."
                            }
                        },
                        max_tokens = 50,
                        temperature = 0.0
                    }
                ),
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(resultContent);
                var answer = jsonResponse["choices"]?.First?["message"]?["content"]?.ToString().Trim().ToLower();

                return answer == "yes";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {errorContent}");
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
    }
}
