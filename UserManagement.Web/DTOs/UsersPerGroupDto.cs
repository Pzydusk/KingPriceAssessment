namespace UserManagement.Web.DTOs
{
    public class UsersPerGroupDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public int UserCount { get; set; }
    }
}
