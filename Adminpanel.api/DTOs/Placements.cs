using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;

namespace Adminpanel.api.DTOs
{
    public class Placements
    {
        // Dtos/Placements/PlacementItemDto.cs
        public record PlacementItemDto(
            Guid? Id, string Position, string PathPattern, string ContentType,
            Guid? PageId, Guid? BlogId, string? ExternalUrl, int SortOrder);

        // Dtos/Placements/PlacementCreateDto.cs
        public class PlacementCreateDto
        {
            public string Position { get; set; } = "Bottom";      // Bottom|Sidebar|Header
            public string PathPattern { get; set; } = default!;    // "/tools/*" or exact path
            public string ContentType { get; set; } = "Page";      // Page|Blog|External
            public Guid? PageId { get; set; }
            public Guid? BlogId { get; set; }
            public string? ExternalUrl { get; set; }
            public int SortOrder { get; set; }
            public bool Active { get; set; } = true;
            public DateTime? StartAt { get; set; }
            public DateTime? EndAt { get; set; }
        }

        // Dtos/Placements/PlacementUpdateDto.cs
        public class PlacementUpdateDto : PlacementCreateDto
        {
            public Guid Id { get; set; }
        }

        // DTOs/Placements.cs
        public record PlacementResolvedDto(
            Guid PlacementId,
            string Type,                // "page" | "blog" | "external"
            string Position,            // Bottom | Sidebar | Header
            string PathPattern,
            int SortOrder,
            Guid? PageId,
            Guid? BlogId,
            string? Name,               // for page
            string? Title,              // for blog
            string? Slug,
            string? RedirectUrl,
            string? Url                 // for external
        );

    }
}
