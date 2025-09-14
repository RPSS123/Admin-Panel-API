using Adminpanel.api.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using static Adminpanel.api.DTOs.Blogs;

public class BlogCreateDtoValidator : AbstractValidator<BlogCreateDto>
{
    public BlogCreateDtoValidator(AdminpanelDbContext db)
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(160)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Use kebab-case, e.g. 'how-to-start'")
            .MustAsync(async (slug, ct) => !await db.BlogPosts.AnyAsync(b => b.Slug == slug, ct))
            .WithMessage("Slug already exists.");
        RuleFor(x => x.Excerpt).MaximumLength(400);
    }
}

public class BlogUpdateDtoValidator : AbstractValidator<BlogUpdateDto>
{
    public BlogUpdateDtoValidator(AdminpanelDbContext db)
    {
        RuleFor(x => x.Id).NotEmpty();
        Include(new BlogCreateDtoValidator(db)); // reuse basic rules
        RuleFor(x => x.Status).Must(s => s is "Draft" or "Published")
            .WithMessage("Status must be Draft or Published.");

        // unique slug (ignore current record)
        RuleFor(x => x).MustAsync(async (dto, ct) =>
            !await db.BlogPosts.AnyAsync(b => b.Slug == dto.Slug && b.Id != dto.Id, ct))
            .WithMessage("Slug already exists for another blog.");
    }
}
