using System;
using System.Linq;
using EnlightDenBackendAPI.Entities;
using static ApplicationDbContext;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/Question")]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _context.Questions
                .Select(q => new GetQuestionDto
                {
                    Id = q.Id,
                    Request = q.Request,
                    Answer = q.Answer,
                    QuestionType = q.QuestionType,
                    ClassId = q.ClassId,
                    StudyToolId = q.StudyToolId
                })
                .ToListAsync();

            return Ok(questions);
        }

        [HttpGet("{id}")]
        public IActionResult GetQuestionById(Guid id)
        {
            var QuestionToGet = _context.Questions.Find(id);

            if (QuestionToGet == null)
            {
                return NotFound();
            }

            return Ok(QuestionToGet);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto createDto)
        {
            var classEntity = await _context.Classes.FindAsync(createDto.ClassId);
            if (classEntity == null)
            {
                return NotFound("Class not found.");
            }

            
            var studyToolEntity = await _context.StudyTools.FindAsync(createDto.StudyToolId);
            if (studyToolEntity == null)
            {
                return NotFound("StudyTool not found.");
            }
            

            var questionToCreate = new Question
            {
                Request = createDto.Request,
                Answer = createDto.Answer,
                QuestionType = createDto.QuestionType,
                ClassId = createDto.ClassId,
                StudyToolId = createDto.StudyToolId
            };

            _context.Questions.Add(questionToCreate);
            await _context.SaveChangesAsync();

            var questionToReturn = new GetQuestionDto
            {
                Request = questionToCreate.Request,
                Answer = questionToCreate.Answer,
                QuestionType = questionToCreate.QuestionType,
                ClassId = questionToCreate.ClassId,
                //StudyToolId = questionToCreate.StudyToolId,
            };

            return CreatedAtAction(nameof(GetQuestionById), new { id = questionToCreate.Id }, questionToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateQuestionDto updateDto, Guid id)
        {
            var questionToUpdate = await _context.Questions.FindAsync(id);

            if (questionToUpdate == null)
            {
                return NotFound();
            }

            questionToUpdate.Request = updateDto.Request;
            questionToUpdate.Answer = updateDto.Answer;
            questionToUpdate.QuestionType = updateDto.QuestionType;
            
            await _context.SaveChangesAsync();

            return Ok(new { Class = questionToUpdate, Message = "Question updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var questionToDelete = _context.Questions.FirstOrDefault(q => q.Id == id);

            if (questionToDelete == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(questionToDelete);
            _context.SaveChanges();

            return Ok("Question deleted");
        }


    }
}

