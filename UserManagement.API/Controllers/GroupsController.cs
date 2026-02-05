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
    public class GroupsController : ControllerBase
    {
        private readonly UserManagementContext _context;

        public GroupsController(UserManagementContext context)
        {
            _context = context;
        }

        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetAll()
        {
            var groups = await _context.Groups
                .OrderBy(g => g.Name)
                .ToListAsync();

            return Ok(groups.Select(g => g.ToDto()).ToList());
        }

        // GET: api/groups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetById([FromRoute] Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            return Ok(group.ToDto());
        }

        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<GroupDto>> Create([FromBody] GroupDto dto)
        {
            if (dto.Id != null)
                return BadRequest("Group Already Exists");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Group name is required.");

            var nameExists = await _context.Groups.AnyAsync(g => g.Name == dto.Name);
            if (nameExists)
                return Conflict("A group with this name already exists.");

            var newGroup = dto.ToEntity();
            newGroup.CreatedDate = DateTime.UtcNow;
            newGroup.UpdatedDate = DateTime.UtcNow;
            newGroup.IsDeleted = false;

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return Ok(newGroup.ToDto());
        }

        // PUT: api/groups
        [HttpPut]
        public async Task<ActionResult<GroupDto>> Update( [FromBody] GroupDto dto)
        {

            if (dto.Id == null)
                return BadRequest("Id is required for update.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Group name is required.");

            var nameExists = await _context.Groups.AnyAsync(g => g.Id != dto.Id && g.Name == dto.Name);
            if (nameExists)
                return Conflict("A group with this name already exists.");

            var group = await _context.Groups.FindAsync(dto.Id);
            if (group == null) return NotFound();

            group.Name = dto.Name;
            group.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(group.ToDto());
        }

        // DELETE: api/groups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
