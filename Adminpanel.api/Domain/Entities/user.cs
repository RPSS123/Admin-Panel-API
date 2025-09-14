using static Adminpanel.api.Domain.Enum.ContentEnum;

namespace Adminpanel.api.Domain.Entities
{
    public class user
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.Editor;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
