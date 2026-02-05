namespace UserManagement.API.Models
{
    public class Permission : BaseClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
