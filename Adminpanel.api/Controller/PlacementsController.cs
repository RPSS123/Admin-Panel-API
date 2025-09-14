using Adminpanel.api.Data;
using Adminpanel.api.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Adminpanel.api.Domain.Enum.ContentEnum;
using static Adminpanel.api.DTOs.Placements;

[ApiController]
[Route("api/placements")]
public class PlacementsController : ControllerBase
{
    private readonly AdminpanelDbContext _db;
    private readonly IMapper _m;

    public PlacementsController(AdminpanelDbContext db, IMapper m) { _db = db; _m = m; }

    // ADMIN LIST (optional filters)
    [Authorize(Roles = "Editor,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlacementItemDto>>> List([FromQuery] string? position, CancellationToken ct = default)
    {
        var q = _db.ContentPlacements.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(position))
            q = q.Where(x => x.Position.ToString() == position);
        var rows = await q.OrderBy(x => x.SortOrder).ThenByDescending(x => x.CreatedAt).ToListAsync(ct);
        return Ok(rows.Select(_m.Map<PlacementItemDto>));
    }

    // CREATE
    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<PlacementItemDto>> Create([FromBody] PlacementCreateDto dto, CancellationToken ct)
    {
        var e = _m.Map<ContentPlacement>(dto);
        _db.ContentPlacements.Add(e);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, _m.Map<PlacementItemDto>(e));
    }

    // GET by id
    [Authorize(Roles = "Editor,Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlacementItemDto>> GetById(Guid id, CancellationToken ct)
    {
        var e = await _db.ContentPlacements.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? NotFound() : _m.Map<PlacementItemDto>(e);
    }

    // UPDATE
    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlacementItemDto>> Update(Guid id, [FromBody] PlacementUpdateDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Route id and body id must match.");

        var e = await _db.ContentPlacements.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return NotFound();

        _m.Map(dto, e);
        await _db.SaveChangesAsync(ct);
        return Ok(_m.Map<PlacementItemDto>(e));
    }

    // DELETE
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var e = await _db.ContentPlacements.FindAsync(new object?[] { id }, ct);
        if (e is null) return NotFound();
        _db.ContentPlacements.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // PUBLIC: resolve attachments for a tool page
    // GET /api/placements/resolve?path=/tools/merge-pdfs&position=bottom
    [AllowAnonymous]
    [HttpGet("resolve")]
    public async Task<ActionResult<IEnumerable<object>>> Resolve([FromQuery] string path, [FromQuery] string position, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(position))
            return BadRequest("path and position are required.");

        var now = DateTime.UtcNow;
        var posEnum = Enum.Parse<PlacementPosition>(position, true);

        // 1) DB filter (cheap)
        var q = _db.ContentPlacements.AsNoTracking()
            .Where(p => p.Active && p.Position == posEnum &&
                        (!p.StartAt.HasValue || p.StartAt <= now) &&
                        (!p.EndAt.HasValue || now <= p.EndAt));

        var candidates = await q.ToListAsync(ct);

        // 2) In-memory wildcard match
        var matched = candidates.Where(p => PathPatternMatcher.IsMatch(p.PathPattern, path))
                                .OrderBy(p => p.SortOrder)
                                .ThenByDescending(p => p.CreatedAt)
                                .ToList();

        // 3) Hydrate minimal data for UI (only published content)
        var results = new List<object>();
        foreach (var p in matched)
        {
            if (p.ContentType == PlacementContentType.Page && p.PageId.HasValue)
            {
                var page = await _db.Pages.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == p.PageId && x.Status == PageStatus.Published, ct);
                if (page != null)
                    results.Add(new
                    {
                        PlacementId = p.Id,
                        p.Position,
                        p.PathPattern,
                        Type = "page",
                        PageId = page.Id,
                        page.Name,
                        page.Slug,
                        page.RedirectUrl,
                        p.SortOrder
                    });
            }
            else if (p.ContentType == PlacementContentType.Blog && p.BlogId.HasValue)
            {
                var blog = await _db.BlogPosts.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == p.BlogId && x.Status == PostStatus.Published, ct);
                if (blog != null)
                    results.Add(new
                    {
                        PlacementId = p.Id,
                        p.Position,
                        p.PathPattern,
                        Type = "blog",
                        BlogId = blog.Id,
                        Title = blog.Title,
                        blog.Slug,
                        p.SortOrder
                    });
            }
            else if (p.ContentType == PlacementContentType.External && !string.IsNullOrWhiteSpace(p.ExternalUrl))
            {
                results.Add(new
                {
                    PlacementId = p.Id,
                    p.Position,
                    p.PathPattern,
                    Type = "external",
                    Url = p.ExternalUrl,
                    p.SortOrder
                });
            }
        }

        return Ok(results);
    }
}
