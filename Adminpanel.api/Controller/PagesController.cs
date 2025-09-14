using Adminpanel.api.Data;
using Adminpanel.api.Domain.Entities;
using Adminpanel.api.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Adminpanel.api.DTOs.Pages;

[ApiController]
[Route("api/pages")]
public class PagesController : ControllerBase
{
    private readonly AdminpanelDbContext _db;
    private readonly IMapper _m;
    public PagesController(AdminpanelDbContext db, IMapper m) { _db = db; _m = m; }

    // GET with basic paging/search
    [HttpGet]
    public async Task<ActionResult<PagedResult<PageListDto>>> List([FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "name", [FromQuery] string? sortDir = "asc",
        CancellationToken ct = default)
    {
        var q = _db.Pages.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => p.Name.Contains(search) || p.Slug.Contains(search));

        var total = await q.CountAsync(ct);

        q = q.OrderBySafe(sortBy, sortDir,
            ("name", p => p.Name),
            ("createdAt", p => p.CreatedAt));

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new PagedResult<PageListDto>(items.Select(_m.Map<PageListDto>).ToList(), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PageDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var e = await _db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        return e is null ? NotFound() : _m.Map<PageDetailDto>(e);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<PageDetailDto>> Create([FromBody] PageCreateDto dto, CancellationToken ct)
    {
        var e = _m.Map<Page>(dto);
        _db.Pages.Add(e);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = e.Id }, _m.Map<PageDetailDto>(e));
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PageDetailDto>> Update(Guid id, [FromBody] PageUpdateDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Route id and body id must match.");

        var e = await _db.Pages.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (e is null) return NotFound();

        _m.Map(dto, e);
        await _db.SaveChangesAsync(ct);
        return Ok(_m.Map<PageDetailDto>(e));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var e = await _db.Pages.FindAsync(new object?[] { id }, ct);
        if (e is null) return NotFound();
        _db.Pages.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
