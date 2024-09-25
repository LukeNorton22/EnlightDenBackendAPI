using System;
using System.Linq;
using System.Threading.Tasks;
using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class ClassController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet] // READ ALL
        public IActionResult GetAll()
        {
            var classes = _context.Classes
                .Select(c => new GetClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId
                }).ToList();

            return Ok(classes);
        }

        [HttpGet("{id}")]
        public IActionResult GetClassById(Guid id)
        {
            var classToGet = _context.Classes.Find(id);

            if (classToGet == null)
            {
                return NotFound();
            }

            return Ok(classToGet);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassDto createDto)
        {
            var user = await _userManager.FindByIdAsync(createDto.UserId.ToString());

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Check for duplicate class name
            if (await _context.ClassNameExistsForUserAsync(createDto.Name, createDto.UserId))
            {
                return BadRequest("A class with this name already exists for this user.");
            }

            var classToCreate = new Class
            {
                Name = createDto.Name,
                Description = createDto.Description,
                UserId = createDto.UserId
            };
            _context.Classes.Add(classToCreate);
            await _context.SaveChangesAsync();

            var classToReturn = new GetClassDto
            {
                Id = classToCreate.Id,
                Name = classToCreate.Name,
                Description = classToCreate.Description,
                UserId = classToCreate.UserId
            };
            return Ok(classToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] UpdateClassDto updateDto, Guid id)
        {
            var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == id);

            if (classToUpdate == null)
            {
                return NotFound();
            }

            classToUpdate.Name = updateDto.Name;
            classToUpdate.Description = updateDto.Description;

            _context.SaveChanges();

            var classToReturn = new GetClassDto
            {
                Id = classToUpdate.Id,
                Name = classToUpdate.Name,
                Description = classToUpdate.Description,
                UserId = classToUpdate.UserId
            };

            return Ok(new { Class = classToReturn, Message = "Class updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var classToDelete = _context.Classes.FirstOrDefault(c => c.Id == id);
            var className = _context.Classes.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefault();

            if (classToDelete == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classToDelete);
            _context.SaveChanges();

            return Ok($"The class named {className} has been deleted");
        }
    }
}
