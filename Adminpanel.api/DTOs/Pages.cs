namespace Adminpanel.api.DTOs
{
    public class Pages
    {
        // Dtos/Pages/PageListDto.cs
        public record PageListDto(
            Guid? Id, string Name, string Slug, bool PublishOnWebsite, bool ShowInMenu, string? RedirectUrl);

        // Dtos/Pages/PageDetailDto.cs
        public record PageDetailDto(
            Guid? Id, string Name, string Slug, string? ContentHtml, string? ImageUrl,
            bool PublishOnWebsite, bool ShowInMenu, string? RedirectUrl, string Status,
            DateTime CreatedAt, DateTime? UpdatedAt);

        // Dtos/Pages/PageCreateDto.cs
        public class PageCreateDto
        {
            public string Name { get; set; } = default!;
            public string Slug { get; set; } = default!;
            public string? ContentHtml { get; set; }
            public string? ImageUrl { get; set; }
            public bool PublishOnWebsite { get; set; }
            public bool ShowInMenu { get; set; }
            public string? RedirectUrl { get; set; } // either this OR content
        }

        // Dtos/Pages/PageUpdateDto.cs
        public class PageUpdateDto : PageCreateDto
        {
            public Guid? Id { get; set; }
            public string Status { get; set; } = "Draft";
        }

    }
}
