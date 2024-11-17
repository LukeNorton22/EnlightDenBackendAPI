using EnlightDenBackendAPI.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnlightDenBackendAPI.Controllers.Helpers
{
    public class StudyModuleHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly string _openAiApiKey;

        public StudyModuleHelper(HttpClient httpClient, ApplicationDbContext context, string openAiApiKey)
        {
            _httpClient = httpClient;
            _context = context;
            _openAiApiKey = openAiApiKey ?? throw new ArgumentNullException(nameof(openAiApiKey));
        }

        public async Task<StudyModule> CreateStudyModuleFromNoteAsync(string noteContent, string mainTopic, StudyTool studyTool, Guid MindMapTopicId, Guid MindMapId)
        {
            var mindMapTopic = await _context.MindMapTopics.FindAsync(MindMapTopicId);
            var subTopics = await GenerateSubTopicsAsync(noteContent, mainTopic);
            var mindMapId = MindMapId;

            var studyModule = new StudyModule
            {
                Id = Guid.NewGuid(),
                MindMapId = MindMapId,
                MindMapTopicId = MindMapTopicId,
                MindMapTopic = mindMapTopic = null!, 
                MindMapTopicName = mainTopic,
                SubTopics = subTopics,
                StudyToolId = studyTool.Id,
                StudyTool = studyTool,   
            };

            // Update the StudyModule reference in SubTopics 
            foreach (var subTopic in subTopics)
            {
                subTopic.StudyModule = studyModule;
                subTopic.StudyModuleId = studyModule.Id;
            }

            return studyModule;
        }

        private async Task<List<SubTopic>> GenerateSubTopicsAsync(string noteContent, string mainTopic)
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
                        content = @"
You are an expert educational assistant tasked with generating highly specific subtopics from the provided main topic
and notes. Your job is to create subtopics with corresponding content that give greater detailed information on that subtopic. 
The output must strictly follow the given format, and irrelevant or off-topic content should be excluded entirely.",
                    },
                    new
                    {
                        role = "user",
                        content = $@"
The main topic for these subtopics is: **'{mainTopic}'**.
Use the most relevant content from the following notes to create **a set of subtopics with detailed content**.
Each subtopic must relate directly to the main topic.

Here are the **strict instructions**:
1. Only create subtopics that relate **directly with the main topic** '{mainTopic}'.
2. Ignore any content that does not relate to the main topic precisely.
3. Ensure each subtopic has a **title** and **detailed content**.
4. Format the output like this:

   SubTopic: [Your subtopic title here]  
   Content: [The detailed content for the subtopic]

5. Do not include any text outside of the **SubTopic: and Content:** format.  
6. Every subtopic and its content must be concise, accurate, and directly drawn from the provided notes.

Below are the notes you should use:

{noteContent}
",
                    },
                        },
                        max_tokens = 4096,
                        temperature = 0.2,
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

                if (choices == null)
                {
                    throw new InvalidOperationException("The response content is null.");
                }

                var subTopics = new List<SubTopic>();
                var lines = choices.Split('\n');
                SubTopic? currentSubTopic = null;

                foreach (var line in lines)
                {
                    if (line.StartsWith("SubTopic:"))
                    {
                        if (currentSubTopic != null)
                        {
                            subTopics.Add(currentSubTopic);
                        }
                        currentSubTopic = new SubTopic
                        {
                            Id = Guid.NewGuid(),
                            Title = line.Substring("SubTopic:".Length).Trim(),
                            Content = string.Empty,
                            StudyModule = null, // This will be set later
                            StudyModuleId = Guid.Empty // This will be set later
                        };
                    }
                    else if (line.StartsWith("Content:") && currentSubTopic != null)
                    {
                        currentSubTopic.Content = line.Substring("Content:".Length).Trim();
                    }
                }

                if (currentSubTopic != null)
                {
                    subTopics.Add(currentSubTopic);
                }

                return subTopics;
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