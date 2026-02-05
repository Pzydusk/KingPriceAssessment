using UserManagement.API.DTOs;
using UserManagement.API.Models;

namespace UserManagement.API.Mapper
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Groups = user.Groups
                    .Select(g => g.ToDto())
                    .ToList()
            };
        }

        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                Id = dto.Id.HasValue ? dto.Id.Value : Guid.NewGuid(),
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                CreatedDate = dto.CreatedDate,
                UpdatedDate = dto.UpdatedDate
            };
        }
    }
}
