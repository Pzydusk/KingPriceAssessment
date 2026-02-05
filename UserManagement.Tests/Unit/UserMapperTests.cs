using UserManagement.API.DTOs;
using UserManagement.API.Mapper;
using Xunit;

namespace UserManagement.Tests.Unit
{
    public class UserMapperTests
    {
        [Fact]
        public void ToEntity_CreatesNewGuid_WhenIdIsNull()
        {
            var dto = new UserDto
            {
                Id = null,
                Name = "Test",
                Surname = "User",
                Email = "test@test.com"
            };

            var entity = dto.ToEntity();

            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal("Test", entity.Name);
        }
    }
}
