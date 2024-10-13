using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnlightDenBackendAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/StudyPlans")]
    public class StudyPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudyPlanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanDto createStudyPlanDto)
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

            var studyPlan = new StudyPlan
            {
                Name = createStudyPlanDto.Name,
                Description = createStudyPlanDto.Description,
                Day = createStudyPlanDto.Day,
                Month = createStudyPlanDto.Month,
                StartTime = createStudyPlanDto.StartTime,
                EndTime = createStudyPlanDto.EndTime,
                UserId = user.Id,
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

        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetStudyPlansForUser()
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

            // Find the user
            var user = await _userManager.FindByIdAsync(userIdClaim);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Fetch all classes associated with the user
            var userClasses = _context
                .Classes.Where(c => c.UserId == user.Id)
                .Select(c => new GetClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId,
                })
                .ToList();

            return Ok(userClasses);
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