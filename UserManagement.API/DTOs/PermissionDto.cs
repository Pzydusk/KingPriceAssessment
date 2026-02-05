using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs
{
    public class PermissionDto
    {
        public Guid? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public Guid GroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
