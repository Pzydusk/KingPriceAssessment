namespace UserManagement.API.DTOs
{
    public class UsersPerGroupDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public int UserCount { get; set; }
    }
}
