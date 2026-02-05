using Microsoft.AspNetCore.Mvc;
using UserManagement.API.DTOs;
using UserManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Mapper;
using UserManagement.API.Context;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManagementContext _context;

        public UsersController(UserManagementContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Groups)
                .OrderBy(u => u.Name)
                .ToListAsync();
            
            return users.Select(u => u.ToDto()).ToList();
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(user.ToDto());
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto dto)
        {
            try
            {

                if (dto.Id != null)
                    return BadRequest("User already exists.");

                if (string.IsNullOrWhiteSpace(dto.Name) ||
                    string.IsNullOrWhiteSpace(dto.Surname) ||
                    string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Name, surname and email are required.");

                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
                if (emailExists)
                    return Conflict("A user with this email already exists.");

                var newUser = dto.ToEntity();
                newUser.CreatedDate = DateTime.UtcNow;
                newUser.UpdatedDate = DateTime.UtcNow;
                newUser.IsDeleted = false;

                var groupIds = dto.Groups?
        .Where(g => g.Id.HasValue)
        .Select(g => g.Id!.Value)
        .Distinct()
        .ToList() ?? new List<Guid>();

                if (groupIds.Count > 0)
                {
                    var groups = await _context.Groups
                        .Where(g => groupIds.Contains(g.Id))
                        .ToListAsync();

                    if (groups.Count != groupIds.Count)
                        return BadRequest("One or more groups do not exist.");

                    foreach (var g in groups)
                        newUser.Groups.Add(g);
                }

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(newUser.ToDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/users/
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update([FromBody] UserDto dto)
        {
            if (dto.Id == null)
                return BadRequest("Id is required for update.");

            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Name, surname and email are required.");

            var user = await _context.Users
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (user == null) return NotFound();

            var emailExists = await _context.Users.AnyAsync(u => u.Id != dto.Id && u.Email == dto.Email);
            if (emailExists)
                return Conflict("A user with this email already exists.");

            user.UpdatedDate = DateTime.UtcNow;
            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.Groups.Clear();

            var groupIds = dto.Groups?
                .Where(g => g.Id != null)
                .Select(g => g.Id!.Value)
                .Distinct()
                .ToList() ?? new List<Guid>();

            if (groupIds.Count > 0)
            {
                var groups = await _context.Groups
                    .Where(g => groupIds.Contains(g.Id))
                    .ToListAsync();

                if (groups.Count != groupIds.Count)
                    return BadRequest("One or more group IDs do not exist.");

                foreach (var g in groups)
                    user.Groups.Add(g);
            }

            await _context.SaveChangesAsync();
            return Ok(user.ToDto());
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/users/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalUserCount()
        {
            var count = await _context.Users.CountAsync();
            return Ok(count);
        }

        // GET: api/users/perGroup
        [HttpGet("perGroup")]
        public async Task<ActionResult<List<UsersPerGroupDto>>> GetUsersPerGroup()
        {
            var result = await _context.Groups
                .Select(g => new UsersPerGroupDto
                {
                    GroupId = g.Id,
                    GroupName = g.Name,
                    UserCount = g.Users.Count
                })
                .OrderBy(x => x.GroupName)
                .ToListAsync();

            return Ok(result);
        }
    }
}
