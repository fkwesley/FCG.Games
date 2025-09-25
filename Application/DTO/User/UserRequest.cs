using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTO.User
{
    public class UserRequest
    {
        [Required]
        [MaxLength(20)]
        public required string UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
