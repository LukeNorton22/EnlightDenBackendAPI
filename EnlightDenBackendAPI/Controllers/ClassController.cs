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


namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class ClassController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet] //READ ALL
        public IActionResult GetAll()
        {
            var Class = _context.Classes
                .Select(Classes => new GetClassDto
                {
                    Id = Classes.Id,
                    Name = Classes.Name,
                    Description = Classes.Description,
                    User = Classes.User,
                    UserId = Classes.UserId

                }).ToList();

            return Ok(Class);
        }

        [HttpGet("{id}")]
        public IActionResult GetClassById(Guid id)
        {
            var ClassToGet = _context.Classes.Find(id);

            if (ClassToGet == null)
            {
                return NotFound();
            }

            return Ok(ClassToGet);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassDto createDto)
        {
            var user = await _context.Users.FindAsync(createDto.UserId);


            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var existingClass = _context.Classes.FirstOrDefault(c => c.Name == createDto.Name && c.UserId == createDto.UserId);

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


        [HttpPut("Id")]
        public IActionResult Update([FromBody] UpdateClassDto updateDto, Guid Id)
        {
            var ClassToUpdate = _context.Set<Class>().FirstOrDefault(Class => Class.Id == Id);

            if (ClassToUpdate == null)
            {
                return BadRequest();
            }

            ClassToUpdate.Name = updateDto.Name;
            ClassToUpdate.Description = updateDto.Description;

            _context.SaveChanges();

            var ClassToReturn = new GetClassDto
            {
                Id = ClassToUpdate.Id,
                Name = ClassToUpdate.Name,
                Description = ClassToUpdate.Description,
                User = ClassToUpdate.User,
                UserId = ClassToUpdate.UserId
            };

            return Ok(new { Class = ClassToReturn, Message = "Class updated successfully" });

        }

        [HttpDelete("Id")]
        public IActionResult Delete(Guid Id)
        {
            var ClassToDelete = _context.Set<Class>().FirstOrDefault(Class => Class.Id == Id);
            var className = _context.Set<Class>().Where(Class => Class.Id == Id).Select(Class => Class.Name).FirstOrDefault();
            var response = $"The class named {className} has been deleted";

            if (ClassToDelete == null)
            {
                return BadRequest();
            }

            _context.Set<Class>().Remove(ClassToDelete);
            _context.SaveChanges();

            return Ok(response);
        }
    }
}
