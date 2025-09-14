using FluentValidation;
using static Adminpanel.api.DTOs.Placements;

public class PlacementCreateDtoValidator : AbstractValidator<PlacementCreateDto>
{
    public PlacementCreateDtoValidator()
    {
        RuleFor(x => x.Position).Must(p => p is "Bottom" or "Sidebar" or "Header")
            .WithMessage("Position must be Bottom, Sidebar or Header.");

        RuleFor(x => x.ContentType).Must(t => t is "Page" or "Blog" or "External")
            .WithMessage("ContentType must be Page, Blog or External.");

        RuleFor(x => x.PathPattern)
            .NotEmpty().MaximumLength(255)
            .Matches(@"^\/.*").WithMessage("Path must start with '/'")
            .Matches(@"^[A-Za-z0-9\-\/\*\_\.\%]+$").WithMessage("Invalid characters in path.");

        // Exactly one of PageId, BlogId, ExternalUrl must be provided
        RuleFor(x => x).Must(dto =>
        {
            var choices = new[]
            {
                dto.PageId.HasValue,
                dto.BlogId.HasValue,
                !string.IsNullOrWhiteSpace(dto.ExternalUrl)
            };
            return choices.Count(c => c) == 1;
        }).WithMessage("Provide exactly one: PageId, BlogId, or ExternalUrl.");

        // If External
        When(x => x.ContentType == "External", () =>
        {
            RuleFor(x => x.ExternalUrl)
                .NotEmpty()
                .Must(u => Uri.TryCreate(u, UriKind.Absolute, out _))
                .WithMessage("ExternalUrl must be a valid absolute URL.");
        });

        // Start < End (if both given)
        RuleFor(x => x).Must(dto =>
            !(dto.StartAt.HasValue && dto.EndAt.HasValue) ||
             dto.StartAt < dto.EndAt)
            .WithMessage("StartAt must be before EndAt.");
    }
}

public class PlacementUpdateDtoValidator : AbstractValidator<PlacementUpdateDto>
{
    public PlacementUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        Include(new PlacementCreateDtoValidator());
    }
}
