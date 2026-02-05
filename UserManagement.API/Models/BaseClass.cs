namespace UserManagement.API.Models
{
    public abstract class BaseClass
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
