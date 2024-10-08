using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/studyplans")]
    [Authorize]
    public class StudyPlanController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudyPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanDto createStudyPlanDto)
        {
            var user = await _context.Users.FindAsync(createStudyPlanDto.UserId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var studyPlan = new StudyPlan
            {
                Name = createStudyPlanDto.Name,
                Description = createStudyPlanDto.Description,
                Day = createStudyPlanDto.Day,
                Month = createStudyPlanDto.Month,
                StartTime = createStudyPlanDto.StartTime,
                EndTime = createStudyPlanDto.EndTime,
                UserId = createStudyPlanDto.UserId
            };

            _context.StudyPlans.Add(studyPlan);
            await _context.SaveChangesAsync();

            var response = new GetStudyPlanDto
            {
                Id = studyPlan.Id,
                Name = studyPlan.Name,
                Description = studyPlan.Description,
                Day = studyPlan.Day,
                Month = studyPlan.Month,
                StartTime = studyPlan.StartTime,
                EndTime = studyPlan.EndTime,
                UserId = studyPlan.UserId
            };

            return CreatedAtAction(nameof(GetStudyPlanById), new { id = studyPlan.Id }, response);
        }

        [HttpGet]
        public IActionResult GetAllStudyPlans()
        {
            var studyPlans = _context.StudyPlans
                .Select(studyPlan => new GetStudyPlanDto
                {
                    Id = studyPlan.Id,
                    Name = studyPlan.Name,
                    Description = studyPlan.Description,
                    Day = studyPlan.Day,
                    Month = studyPlan.Month,
                    StartTime = studyPlan.StartTime,
                    EndTime = studyPlan.EndTime,
                    UserId = studyPlan.UserId
                })
                .ToList();
            return Ok(studyPlans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudyPlanById(Guid id)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(id);

            if (studyPlan == null)
            {
                return NotFound();
            }

            var response = new GetStudyPlanDto
            {
                Id = studyPlan.Id,
                Name = studyPlan.Name,
                Description = studyPlan.Description,
                Day = studyPlan.Day,
                Month = studyPlan.Month,
                StartTime = studyPlan.StartTime,
                EndTime = studyPlan.EndTime,
                UserId = studyPlan.UserId
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyPlan(Guid id, [FromBody] UpdateStudyPlanDto updateStudyPlanDto)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(id);

            if (studyPlan == null)
            {
                return NotFound();
            }

            studyPlan.Name = updateStudyPlanDto.Name;
            studyPlan.Description = updateStudyPlanDto.Description;
            studyPlan.Day = updateStudyPlanDto.Day;
            studyPlan.Month = updateStudyPlanDto.Month;
            studyPlan.StartTime = updateStudyPlanDto.StartTime;
            studyPlan.EndTime = updateStudyPlanDto.EndTime;

            await _context.SaveChangesAsync();

            var response = new GetStudyPlanDto
            {
                Id = studyPlan.Id,
                Name = studyPlan.Name,
                Description = studyPlan.Description,
                Day = studyPlan.Day,
                Month = studyPlan.Month,
                StartTime = studyPlan.StartTime,
                EndTime = studyPlan.EndTime,
                UserId = studyPlan.UserId
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyPlan(Guid id)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(id);

            if (studyPlan == null)
            {
                return NotFound();
            }

            _context.StudyPlans.Remove(studyPlan);
            await _context.SaveChangesAsync();

            return Ok("Study plan deleted.");
        }

    }
}
