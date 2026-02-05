using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
        public List<GroupDto> Groups { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
