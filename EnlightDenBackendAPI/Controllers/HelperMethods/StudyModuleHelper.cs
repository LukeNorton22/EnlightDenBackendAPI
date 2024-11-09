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

        public async Task<StudyModule> CreateStudyModuleFromNoteAsync(string noteContent, string mainTopic, StudyTool studyTool)
        {
            var subTopics = await GenerateSubTopicsAsync(noteContent, mainTopic);
            var practiceTest = await GeneratePracticeTestsAsync(noteContent);

            var studyModule = new StudyModule
            {
                Id = Guid.NewGuid(),
                MainTopic = mainTopic,
                SubTopics = subTopics,
                PracticeTest = practiceTest,
                StudyToolId = studyTool.Id,
                StudyTool = studyTool,
                MindMapId = studyTool.MindMapId
            };

            // Update the StudyModule reference in SubTopics and PracticeTests
            // Update the StudyModule reference in SubTopics and PracticeTest
            foreach (var subTopic in subTopics)
            {
                subTopic.StudyModule = studyModule;
                subTopic.StudyModuleId = studyModule.Id;
            }

            practiceTest.StudyModule = studyModule;
            practiceTest.StudyModuleId = studyModule.Id;

            foreach (var practiceQuestion in practiceTest.PracticeQuestions)
            {
                practiceQuestion.PracticeTest = practiceTest;
                practiceQuestion.PracticeTestId = practiceTest.Id;
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

        private async Task<List<PracticeQuestion>> GeneratePracticeQuestionsAsync(string noteContent)
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
You are an expert educational assistant tasked with generating practice test questions and answers from the provided notes. 
Your job is to create questions that are relevant and directly drawn from the provided notes. 
The output must strictly follow the given format, and irrelevant or off-topic content should be excluded entirely.",
                        },
                        new
                        {
                            role = "user",
                            content = $@"
Use the most relevant content from the following notes to create **a set of practice test questions and answers**. The question types
can be multiple-choice or true/false.
Each question must relate directly to the content of the notes.

Here are the **strict instructions**:
1. Only create questions that relate **directly with the content** of the notes.
2. Ignore any content that does not relate to the notes precisely.
3. Ensure each question has a **question** and **answer** and **question type**.
4. For question types, assign the integer 1 for multiple-choice questions and the integer 2 for true/false questions 
   to the **QuestionType** field.
5. Format the output like this:

   Question: [Your question here]  
   Answer: [The answer here]
   QuestionType: [The question type here]

5. Do not include any text outside of the **Question: and Answer: and QuestionType: ** format.  
6. Every question and its answer must be concise, accurate, and directly drawn from the provided notes.

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

                var practiceQuestions = new List<PracticeQuestion>();
                var lines = choices.Split('\n');
                PracticeQuestion? currentPracticeQuestion = null;

                foreach (var line in lines)
                {
                    if (line.StartsWith("Question:"))
                    {
                        if (currentPracticeQuestion != null)
                        {
                            practiceQuestions.Add(currentPracticeQuestion);
                        }
                        currentPracticeQuestion = new PracticeQuestion
                        {
                            Id = Guid.NewGuid(),
                            Question = line.Substring("Question:".Length).Trim(),
                            Answer = string.Empty,
                            QuestionType = PracticeQuestionType.TrueFalse, // Default value, will be set later
                            PracticeTest = null, // This will be set later
                            PracticeTestId = Guid.Empty // This will be set later
                        };
                    }
                    else if (line.StartsWith("Answer:") && currentPracticeQuestion != null)
                    {
                        currentPracticeQuestion.Answer = line.Substring("Answer:".Length).Trim();
                    }
                    else if (line.StartsWith("QuestionType:") && currentPracticeQuestion != null)
                    {
                        var questionTypeValue = int.Parse(line.Substring("QuestionType:".Length).Trim());
                        currentPracticeQuestion.QuestionType = (PracticeQuestionType)questionTypeValue;
                    }
                }

                if (currentPracticeQuestion != null)
                {
                    practiceQuestions.Add(currentPracticeQuestion);
                }

                return practiceQuestions;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"OpenAI API error: {response.StatusCode} - {errorContent}"
                );
            }
        }

        private async Task<PracticeTest> GeneratePracticeTestsAsync(string noteContent)
        {
            var practiceQuestions = await GeneratePracticeQuestionsAsync(noteContent);

            var practiceTest = new PracticeTest
            {
                Id = Guid.NewGuid(),
                PracticeQuestions = practiceQuestions,
                StudyModule = null, // This will be set later
                StudyModuleId = Guid.Empty // This will be set later
            };

            // Update the PracticeTest reference in PracticeQuestions
            foreach (var practiceQuestion in practiceQuestions)
            {
                practiceQuestion.PracticeTest = practiceTest;
                practiceQuestion.PracticeTestId = practiceTest.Id;
            }

            return practiceTest;
        }




    }
}
