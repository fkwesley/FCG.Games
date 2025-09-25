namespace Application.DTO.User
{
    public class UserResponse
    {
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; } = false;
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
