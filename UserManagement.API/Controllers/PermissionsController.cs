using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Context;
using UserManagement.API.DTOs;
using UserManagement.API.Mapper;
using UserManagement.API.Models;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly UserManagementContext _context;

        public PermissionsController(UserManagementContext context)
        {
            _context = context;
        }

        // GET: api/permissions?groupId=
        [HttpGet]
        public async Task<ActionResult<List<PermissionDto>>> GetAll([FromQuery] Guid? groupId = null)
        {
            var permissions = await _context.Permissions
                .Where(p => groupId == null || p.GroupId == groupId.Value)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Ok(permissions.Select(p => p.ToDto()).ToList());
        }

        // GET: api/permissions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetById([FromRoute]Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return NotFound();

            return Ok(permission.ToDto());
        }

        // POST: api/permissions
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> Create([FromBody]PermissionDto dto)
        {
            if (dto.Id != null)
                return BadRequest("Group already exists");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Permission name is required.");

            var groupExists = await _context.Groups.AnyAsync(g => g.Id == dto.GroupId);
            if (!groupExists) return BadRequest("Group does not exist.");

            var duplicate = await _context.Permissions.AnyAsync(p =>
                p.GroupId == dto.GroupId && p.Name == dto.Name);

            if (duplicate)
                return Conflict("This permission already exists for the group.");

            var newPermission = dto.ToEntity();
            newPermission.CreatedDate = DateTime.UtcNow;
            newPermission.UpdatedDate = DateTime.UtcNow;
            newPermission.IsDeleted = false;

            _context.Permissions.Add(newPermission);
            await _context.SaveChangesAsync();

            return Ok(newPermission.ToDto());
        }

        // PUT: api/permissions
        [HttpPut()]
        public async Task<ActionResult<PermissionDto>> Update([FromBody]PermissionDto dto)
        {
            if (dto.Id == null)
                return BadRequest("Permission does not exist");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Permission name is required.");

            var permission = await _context.Permissions.FindAsync(dto.Id);
            if (permission == null) return NotFound();

            var groupExists = await _context.Groups.AnyAsync(g => g.Id == dto.GroupId);
            if (!groupExists) return BadRequest("Group does not exist.");

            var duplicate = await _context.Permissions.AnyAsync(p =>
                p.Id != dto.Id && p.GroupId == dto.GroupId && p.Name == dto.Name);

            if (duplicate)
                return Conflict("This permission already exists for the group.");

            permission.Name = dto.Name;
            permission.GroupId = dto.GroupId;
            permission.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(permission.ToDto());
        }

        // DELETE: api/permissions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return NotFound();

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
