using Adminpanel.api.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using static Adminpanel.api.DTOs.Pages;

public class PageCreateDtoValidator : AbstractValidator<PageCreateDto>
{
    public PageCreateDtoValidator(AdminpanelDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(160)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .MustAsync(async (slug, ct) => !await db.Pages.AnyAsync(p => p.Slug == slug, ct))
            .WithMessage("Slug already exists.");

        // XOR: either ContentHtml or RedirectUrl, not both/none
        RuleFor(x => x).Must(dto =>
            !string.IsNullOrWhiteSpace(dto.ContentHtml) ^
            !string.IsNullOrWhiteSpace(dto.RedirectUrl))
            .WithMessage("Provide either ContentHtml or RedirectUrl (exactly one).");
    }
}

public class PageUpdateDtoValidator : AbstractValidator<PageUpdateDto>
{
    public PageUpdateDtoValidator(AdminpanelDbContext db)
    {
        RuleFor(x => x.Id).NotEmpty();
        Include(new PageCreateDtoValidator(db));
        RuleFor(x => x.Status).Must(s => s is "Draft" or "Published")
            .WithMessage("Status must be Draft or Published.");

        RuleFor(x => x).MustAsync(async (dto, ct) =>
            !await db.Pages.AnyAsync(p => p.Slug == dto.Slug && p.Id != dto.Id, ct))
            .WithMessage("Slug already exists for another page.");
    }
}
