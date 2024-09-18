using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightDenBackendAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context
                .Users.Select(user => new GetUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            var UserToGet = _context.Set<User>().FirstOrDefault(user => user.Id == id);

            if (UserToGet == null)
            {
                return BadRequest("User not found.");
            }

            return Ok(UserToGet);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = createUserDto.Password,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new GetUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };

            return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromBody] UpdateUserDto updateDto, Guid id)
        {
            var UserToUpdate = _context.Set<User>().FirstOrDefault(user => user.Id == id);

            if (UserToUpdate == null)
            {
                return BadRequest("User not found");
            }

            UserToUpdate.Name = updateDto.Name;
            UserToUpdate.Email = updateDto.Email;

            _context.SaveChanges();

            var UserToReturn = new GetUserDto
            {
                Id = UserToUpdate.Id,
                Name = UserToUpdate.Name,
                Email = UserToUpdate.Email,
            };
            return Ok(UserToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var UserToDelete = _context.Set<User>().FirstOrDefault(user => user.Id == id);

            if (UserToDelete == null)
            {
                return BadRequest("User not found.");
            }

            _context.Set<User>().Remove(UserToDelete);
            _context.SaveChanges();

            return Ok("User Id: " + UserToDelete.Id + " Has Been Deleted");
        }
    }
}
