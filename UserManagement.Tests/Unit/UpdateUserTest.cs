using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Context;
using UserManagement.API.DTOs;
using UserManagement.API.Models;
using Xunit;

namespace UserManagement.Tests.Unit
{
    public class UpdateUserTest
    {
        [Fact]
        public async Task UpdateUser_WorksWithInMemoryDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_UpdateUser")
                .Options;

            using var context = new UserManagementContext(options);

            // Add a user first
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Old",
                Surname = "Name",
                Email = "old@test.com"
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            var dto = new UserDto
            {
                Id = existingUser.Id,
                Name = "Updated",
                Surname = "User",
                Email = "updated@test.com"
            };

            // Act
            var result = await controller.Update(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);

            // Verify update in database
            var updatedUser = await context.Users.FindAsync(existingUser.Id);
            Assert.Equal("Updated", updatedUser.Name);
            Assert.Equal("updated@test.com", updatedUser.Email);
        }
    }
}