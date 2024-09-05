using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/StudyTool")]
    public class StudyToolsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;

        public StudyToolsController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        }

        [HttpPost("GenerateFlashcards")]
        public async Task<IActionResult> GenerateFlashcardsFromPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid PDF file.");
            }

            string extractedText;

            // Use a memory stream to handle the file
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position to the beginning

                // Extract text from the PDF
                extractedText = ExtractTextFromPdf(stream);
            }

            var flashcards = await GenerateFlashcardsAsync(extractedText);

            return Ok(new { Flashcards = flashcards });
        }

        [HttpPost("GenerateTest")]
        public async Task<IActionResult> GenerateTestFromPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid PDF file.");
            }

            string extractedText;

            // Use a memory stream to handle the file
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position to the beginning

                // Extract text from the PDF
                extractedText = ExtractTextFromPdf(stream);
            }

            var testQuestions = await GenerateTestQuestionsAsync(extractedText);

            return Ok(new { TestQuestions = testQuestions });
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder extractedText = new StringBuilder();

            // Open the PDF document using iText7
            using (var pdfReader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                // Loop through all the pages
                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    // Extract text from each page using SimpleTextExtractionStrategy
                    var strategy = new SimpleTextExtractionStrategy();
                    var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                    extractedText.Append(pageText);
                }
            }

            return extractedText.ToString();
        }

        private async Task<List<string>> GenerateFlashcardsAsync(string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful assistant that generates educational flashcards." },
                        new { role = "user", content = $"Create a set of at least 10 detailed flashcards based on the following text, label them Q: and A:: {text}" }
                    },
                    max_tokens = 1500,  // Set max_tokens to 1500 for longer responses
                    temperature = 0.7
                })
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(resultContent);
                var choices = jsonResponse["choices"]?.First?["message"]?["content"]?.ToString();

                // Split by questions and answers and clean up the response
                var flashcards = SplitContentIntoPairs(choices, "Flashcard");

                return flashcards;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {errorContent}");
            }
        }

        private async Task<List<string>> GenerateTestQuestionsAsync(string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful assistant that generates educational test questions and answers from notes." },
                        new { role = "user", content = $"Create a test consisting of at least 10 short-answer and true/false questions based on the following text, label them Q: and A:: {text}" }
                    },
                    max_tokens = 1500,  // Set max_tokens to 1500 for longer responses
                    temperature = 0.7
                })
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(resultContent);
                var choices = jsonResponse["choices"]?.First?["message"]?["content"]?.ToString();

                // Split by questions and answers and clean up the response
                var testQuestions = SplitContentIntoPairs(choices, "Test Question");

                return testQuestions;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {errorContent}");
            }
        }

        private List<string> SplitContentIntoPairs(string text, string itemType)
        {
            var items = new List<string>();
            var parts = text.Split(new[] { "Q: ", "A: " }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i += 2)
            {
                if (i + 1 < parts.Length)
                {
                    // Ensure each pair is formatted as "Q: ... A: ..."
                    var question = parts[i].Trim();
                    var answer = parts[i + 1].Trim();
                    items.Add($"Q: {question} A: {answer}");
                }
            }

            return items;
        }
    }
}
