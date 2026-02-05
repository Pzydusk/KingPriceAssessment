using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Context;
using UserManagement.API.Models;
using Xunit;

namespace UserManagement.Tests.Unit
{
    public class CountEndpointsTest
    {
        [Fact]
        public async Task CountEndpoints_WorkWithInMemoryDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Counts_{Guid.NewGuid()}")
                .Options;

            using var context = new UserManagementContext(options);

            // Add test data with ALL required fields
            var group1 = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var group2 = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Level 1",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Name = "User1",
                Surname = "Test1", // Required field
                Email = "user1@test.com",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Name = "User2",
                Surname = "Test2", // Required field
                Email = "user2@test.com",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            // Manually set up the relationships
            user1.Groups.Add(group1);
            user2.Groups.Add(group1);
            user2.Groups.Add(group2);

            // Add groups to users
            group1.Users.Add(user1);
            group1.Users.Add(user2);
            group2.Users.Add(user2);

            context.Groups.Add(group1);
            context.Groups.Add(group2);
            context.Users.Add(user1);
            context.Users.Add(user2);

            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act 1: Test user count
            var countResult = await controller.GetTotalUserCount();

            // Assert 1
            var countActionResult = Assert.IsType<ActionResult<int>>(countResult);
            var countOkResult = Assert.IsType<OkObjectResult>(countActionResult.Result);
            var userCount = Assert.IsType<int>(countOkResult.Value);
            Assert.Equal(2, userCount);

            // Act 2: Test per group count
            var perGroupResult = await controller.GetUsersPerGroup();

            // Assert 2
            var perGroupActionResult = Assert.IsType<ActionResult<System.Collections.Generic.List<UserManagement.API.DTOs.UsersPerGroupDto>>>(perGroupResult);
            var perGroupOkResult = Assert.IsType<OkObjectResult>(perGroupActionResult.Result);
            var groupCounts = perGroupOkResult.Value as System.Collections.Generic.List<UserManagement.API.DTOs.UsersPerGroupDto>;

            Assert.NotNull(groupCounts);

            // Sort by GroupName to ensure consistent order
            var sortedGroups = groupCounts.OrderBy(g => g.GroupName).ToList();

            Assert.Equal(2, sortedGroups.Count);

            var adminGroup = sortedGroups.First(g => g.GroupName == "Admin");
            var level1Group = sortedGroups.First(g => g.GroupName == "Level 1");

            Assert.Equal(2, adminGroup.UserCount);
            Assert.Equal(1, level1Group.UserCount);
        }
    }
}