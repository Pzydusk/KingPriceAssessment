using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Context;
using UserManagement.API.DTOs;
using Xunit;

namespace UserManagement.Tests.Unit
{
    public class CreateUserTest
    {
        [Fact]
        public async Task CreateUser_WorksWithInMemoryDatabase()
        {
            // Arrange - Use real in-memory database
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateUser")
                .Options;

            using var context = new UserManagementContext(options);
            var controller = new UsersController(context);

            var dto = new UserDto
            {
                Name = "Test",
                Surname = "User",
                Email = "test@test.com"
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var createdUser = Assert.IsType<UserDto>(okResult.Value);

            Assert.Equal("Test", createdUser.Name);
            Assert.Equal("test@test.com", createdUser.Email);

            // Verify it was saved to database
            Assert.Equal(1, await context.Users.CountAsync());
        }
    }
}