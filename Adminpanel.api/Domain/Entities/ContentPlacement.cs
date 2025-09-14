using static Adminpanel.api.Domain.Enum.ContentEnum;

namespace Adminpanel.api.Domain.Entities
{
    public class ContentPlacement
    {
        public Guid Id { get; set; }
        public PlacementPosition Position { get; set; } = PlacementPosition.Bottom;
        public string PathPattern { get; set; } = default!; // '/tools/*' or '/tools/merge-pdfs'
        public PlacementContentType ContentType { get; set; }
        public Guid? PageId { get; set; }
        public Page? Page { get; set; }
        public Guid? BlogId { get; set; }
        public BlogPost? Blog { get; set; }
        public string? ExternalUrl { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; } = true;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
