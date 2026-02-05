using UserManagement.API.DTOs;
using UserManagement.API.Models;

namespace UserManagement.API.Mapper
{
    public static class GroupMapper
    {
        public static GroupDto ToDto(this Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name
            };
        }

        public static Group ToEntity(this GroupDto dto)
        {
            return new Group
            {
                Id = dto.Id.HasValue && dto.Id.Value != Guid.Empty ? dto.Id.Value : Guid.NewGuid(),
                Name = dto.Name,
                CreatedDate = dto.CreatedDate,
                UpdatedDate = dto.UpdatedDate
            };
        }
    }
}
