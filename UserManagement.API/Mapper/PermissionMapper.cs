using UserManagement.API.DTOs;
using UserManagement.API.Models;

namespace UserManagement.API.Mapper
{
    public static class PermissionMapper
    {
        public static PermissionDto ToDto(this Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                GroupId = permission.GroupId
            };
        }

        public static Permission ToEntity(this PermissionDto dto)
        {
            return new Permission
            {
                Id = dto.Id.HasValue && dto.Id.Value != Guid.Empty ? dto.Id.Value : Guid.NewGuid(),
                Name = dto.Name,
                GroupId = dto.GroupId,
                CreatedDate = dto.CreatedDate,
                UpdatedDate = dto.UpdatedDate
            };
        }
    }
}
