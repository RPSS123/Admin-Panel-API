using static Adminpanel.api.Domain.Enum.ContentEnum;

namespace Adminpanel.api.Domain.Entities
{
    public class Page
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? ContentHtml { get; set; }
        public string? ImageUrl { get; set; }
        public bool PublishOnWebsite { get; set; }
        public bool ShowInMenu { get; set; }
        public string? RedirectUrl { get; set; }
        public PageStatus Status { get; set; } = PageStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
