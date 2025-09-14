using static Adminpanel.api.Domain.Enum.ContentEnum;

namespace Adminpanel.api.Domain.Entities
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Excerpt { get; set; }
        public string? ContentHtml { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDisplay { get; set; } = true;
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }
    }
}
