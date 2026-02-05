using System;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Models;


namespace UserManagement.API.Context
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Groups)
                .WithMany(g => g.Users)
                .UsingEntity(j => j.ToTable("UserGroups"));

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Permissions)
                .WithOne(p => p.Group)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var level1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var level2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

            modelBuilder.Entity<Group>().HasData(
                new Group { Id = adminId, Name = "Admin", CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false },
                new Group { Id = level1Id, Name = "Level 1", CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false },
                new Group { Id = level2Id, Name = "Level 2", CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Users.View", GroupId = adminId, CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false },
                new Permission { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Users.Edit", GroupId = adminId, CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false },
                new Permission { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Users.View", GroupId = level1Id, CreatedDate = seedDate, UpdatedDate = seedDate, IsDeleted = false }
            );
        }

    }
}
