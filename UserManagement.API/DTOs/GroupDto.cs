using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs
{
    public class GroupDto
    {
        public Guid? Id { get; set; }        
        [MaxLength(100)]
        public string? Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
