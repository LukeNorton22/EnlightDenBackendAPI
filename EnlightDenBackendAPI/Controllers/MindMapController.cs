using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using EnlightDenBackendAPI.Entities;


namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/MindMap")]
    public class MindMapController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly ApplicationDbContext _context;

        public MindMapController(IConfiguration configuration, ApplicationDbContext context)
        {
            _httpClient = new HttpClient();
            _openAiApiKey = configuration["OpenAI:ApiKey"];
            _context = context;
        }

        [HttpPost("CreateMindMap")]
        public async Task<IActionResult> UploadPdfAndGenerateMindMap(IFormFile file, Guid classId, Guid userId)
        {
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

            // Create new MindMap object
            var mindMap = new MindMap
            {
                Id = Guid.NewGuid(),
                ClassId = classId,
                UserId = userId,
                Topics = new List<MindMapTopic>() 
            };

            foreach (var topic in mindMapTopics)
            {
                var mindMapTopic = new MindMapTopic
                {
                    Id = Guid.NewGuid(),
                    Name = topic,
                    MindMap = mindMap,
                    MindMapId = mindMap.Id
                };

                mindMap.Topics.Add(mindMapTopic); // Add topic to list
                mindMap.TopicIds.Add(mindMapTopic.Id);
            }

            _context.MindMaps.Add(mindMap);
            await _context.SaveChangesAsync();

            return Ok(new { MindMapId = mindMap.Id, MindMapTopics = mindMapTopics });
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
                    var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                    extractedText.Append(pageText);
                }
            }

            return extractedText.ToString();
        }

        private async Task<List<string>> GenerateMindMapTopicsAsync(string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful assistant that generates mind map topics." },
                        new { role = "user", content = $"Extract the main topics from the following text, only return the topic names from this text, and do not number them: {text}" }
                    },
                    max_tokens = 1500,
                    temperature = 0.5
                })
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(resultContent);
                var choices = jsonResponse["choices"]?.First?["message"]?["content"]?.ToString();
                var topics = choices?.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();

                return topics ?? new List<string>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {errorContent}");
            }
        }
    }
}
