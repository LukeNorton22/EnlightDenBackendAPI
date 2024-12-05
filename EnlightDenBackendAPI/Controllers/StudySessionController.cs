using System.Security.Claims;
using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnlightDenBackendAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/StudySession")]
    public class StudySessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudySessionController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudySession(
            [FromBody] CreateStudySessionDto createStudySessionDto
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
                        

            if (user == null )
            {
                return BadRequest("User or Class not found.");
            }
            var StudySession = new StudySession
            {
                Name = createStudySessionDto.Name,
                Description = createStudySessionDto.Description,
                Day = createStudySessionDto.Day,
                Month = createStudySessionDto.Month,
                StartTime = createStudySessionDto.StartTime,
                EndTime = createStudySessionDto.EndTime,
                UserId = user.Id,
               
                ClassName = createStudySessionDto.ClassName

            };

            _context.StudySessions.Add(StudySession);
            await _context.SaveChangesAsync();

            var response = new GetStudySessionDto
            {
                Id = StudySession.Id,
                Name = StudySession.Name,
                Description = StudySession.Description,
                Day = StudySession.Day,
                Month = StudySession.Month,
                StartTime = StudySession.StartTime,
                EndTime = StudySession.EndTime,
                UserId = StudySession.UserId,
                ClassName = StudySession.ClassName
            };

            return CreatedAtAction(nameof(GetStudySessionById), new { id = StudySession.Id }, response);
        }

        [HttpGet]
        public IActionResult GetAllStudySessions()
        {
            var StudySessions = _context
                .StudySessions.Select(StudySession => new GetStudySessionDto
                {
                    Id = StudySession.Id,
                    Name = StudySession.Name,
                    Description = StudySession.Description,
                    Day = StudySession.Day,
                    Month = StudySession.Month,
                    StartTime = StudySession.StartTime,
                    EndTime = StudySession.EndTime,
                    UserId = StudySession.UserId,
                    ClassName = StudySession.ClassName
                })
                .ToList();
            return Ok(StudySessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudySessionById(Guid id)
        {
            var StudySession = await _context.StudySessions.FindAsync(id);

            if (StudySession == null)
            {
                return NotFound();
            }

            var response = new GetStudySessionDto
            {
                Id = StudySession.Id,
                Name = StudySession.Name,
                Description = StudySession.Description,
                Day = StudySession.Day,
                Month = StudySession.Month,
                StartTime = StudySession.StartTime,
                EndTime = StudySession.EndTime,
                UserId = StudySession.UserId,
                ClassName = StudySession.ClassName
            };

            return Ok(response);
        }

        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetStudySessionsForUser()
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
            var userStudySessions = _context
                .StudySessions.Where(c => c.UserId == user.Id)
                .Select(c => new GetStudySessionDto
                {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Day = c.Day,
                Month = c.Month,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                UserId = c.UserId,
                ClassName = c.ClassName
                })
                .ToList();

            return Ok(userStudySessions);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStudySession(
            Guid id,
            [FromBody] UpdateStudySessionDto updateStudySessionDto
        )
        {
            var StudySession = await _context.StudySessions.FindAsync(id);

            if (StudySession == null)
            {
                return NotFound();
            }

            StudySession.Name = updateStudySessionDto.Name;
            StudySession.Description = updateStudySessionDto.Description;
            StudySession.Day = updateStudySessionDto.Day;
            StudySession.Month = updateStudySessionDto.Month;
            StudySession.StartTime = updateStudySessionDto.StartTime;
            StudySession.EndTime = updateStudySessionDto.EndTime;

            await _context.SaveChangesAsync();

            var response = new GetStudySessionDto
            {
                Id = StudySession.Id,
                Name = StudySession.Name,
                Description = StudySession.Description,
                Day = StudySession.Day,
                Month = StudySession.Month,
                StartTime = StudySession.StartTime,
                EndTime = StudySession.EndTime,
                UserId = StudySession.UserId,
                ClassName = StudySession.ClassName
            };

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudySession(Guid id)
        {
            var StudySession = await _context.StudySessions.FindAsync(id);

            if (StudySession == null)
            {
                return NotFound();
            }

            _context.StudySessions.Remove(StudySession);
            await _context.SaveChangesAsync();

            return Ok("Study plan deleted.");
        }
    }
}
