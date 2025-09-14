namespace Adminpanel.api.DTOs
{
    public class Blogs
    {
        // Dtos/Blogs/BlogListDto.cs
        public record BlogListDto(
            Guid? Id, string Title, string Slug, string? Excerpt,
            string? ImageUrl, bool IsDisplay, string Status,
            DateTime CreatedAt, DateTime? PublishedAt);

        // Dtos/Blogs/BlogDetailDto.cs
        public record BlogDetailDto(
            Guid? Id, string Title, string Slug, string? Excerpt,
            string? ContentHtml, string? ImageUrl, bool IsDisplay,
            string Status, DateTime CreatedAt, DateTime? PublishedAt);

        // Dtos/Blogs/BlogCreateDto.cs
        public class BlogCreateDto
        {
            public string Title { get; set; } = default!;
            public string Slug { get; set; } = default!;
            public string? Excerpt { get; set; }
            public string? ContentHtml { get; set; }
            public string? ImageUrl { get; set; }
            public bool IsDisplay { get; set; } = true;
        }

        // Dtos/Blogs/BlogUpdateDto.cs
        public class BlogUpdateDto : BlogCreateDto
        {
            public Guid? Id { get; set; }
            public string Status { get; set; } = "Draft"; // Draft | Published
        }

    }
}
