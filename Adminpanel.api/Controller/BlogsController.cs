using Adminpanel.api.Data;
using Adminpanel.api.Domain.Entities;
using Adminpanel.api.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Adminpanel.api.Domain.Enum.ContentEnum;
using static Adminpanel.api.DTOs.Blogs;

[ApiController]
[Route("api/blogs")]
public class BlogsController : ControllerBase
{
    private readonly AdminpanelDbContext _db;
    private readonly IMapper _m;

    public BlogsController(AdminpanelDbContext db, IMapper m) { _db = db; _m = m; }

    // GET /api/blogs?search=&status=all|draft|published&page=1&pageSize=20&sortBy=createdAt|title&sortDir=desc|asc
    [HttpGet]
    public async Task<ActionResult<PagedResult<BlogListDto>>> List([FromQuery] string? search,
        [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "createdAt", [FromQuery] string? sortDir = "desc",
        CancellationToken ct = default)
    {
        var q = _db.BlogPosts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(b => b.Title.Contains(search));

        if (!string.IsNullOrWhiteSpace(status))
        {
            status = status.ToLowerInvariant();
            if (status == "published") q = q.Where(b => b.Status == PostStatus.Published);
            else if (status == "draft") q = q.Where(b => b.Status == PostStatus.Draft);
        }

        var total = await q.CountAsync(ct);

        q = q.OrderBySafe(sortBy, sortDir,
            ("createdAt", b => b.CreatedAt),
            ("title", b => b.Title));

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var dto = items.Select(_m.Map<BlogListDto>).ToList();

        return Ok(new PagedResult<BlogListDto>(dto, total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BlogDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var entity = await _db.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        return entity is null ? NotFound() : _m.Map<BlogDetailDto>(entity);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<BlogDetailDto>> Create([FromBody] BlogCreateDto dto, CancellationToken ct)
    {
        var e = _m.Map<BlogPost>(dto);
        _db.BlogPosts.Add(e);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = e.Id }, _m.Map<BlogDetailDto>(e));
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BlogDetailDto>> Update(Guid id, [FromBody] BlogUpdateDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Route id and body id must match.");

        var e = await _db.BlogPosts.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (e is null) return NotFound();

        // Map onto existing entity
        _m.Map(dto, e);
        if (e.Status == PostStatus.Published && e.PublishedAt == null)
            e.PublishedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(_m.Map<BlogDetailDto>(e));
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var e = await _db.BlogPosts.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (e is null) return NotFound();

        e.Status = PostStatus.Published;
        e.PublishedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var e = await _db.BlogPosts.FindAsync(new object?[] { id }, ct);
        if (e is null) return NotFound();
        _db.BlogPosts.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
