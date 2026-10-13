using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonClub.Api.Data;
using NeonClub.Shared.Constracts;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Controllers
{
    [ApiController]
    [Route("api/zones")]
    public class ZonesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ZonesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZoneResponseDto>>> GetAll(
            [FromQuery] string? category,
            [FromQuery] string? search)
        {
            var query = _db.Zones.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
                query = query.Where(z => z.Category == category);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(z =>
                    z.Title.ToLower().Contains(s) ||
                    z.Subtitle.ToLower().Contains(s) ||
                    z.Gpu.ToLower().Contains(s));
            }

            var zones = await query.OrderBy(z => z.Price).ToListAsync();

            var images = await LoadImagesByZoneAsync(zones.Select(z => z.Id).ToList());

            return Ok(zones.Select(z =>
                Map(z, images.TryGetValue(z.Id, out var list) ? list : new List<string>())));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ZoneResponseDto>> GetOne(Guid id)
        {
            var zone = await _db.Zones.AsNoTracking().FirstOrDefaultAsync(z => z.Id == id);
            if (zone is null) return NotFound();

            var images = await LoadImagesByZoneAsync(new List<Guid> { zone.Id });
            var list = images.TryGetValue(zone.Id, out var l) ? l : new List<string>();

            return Ok(Map(zone, list));
        }

        private async Task<Dictionary<Guid, List<string>>> LoadImagesByZoneAsync(List<Guid> zoneIds)
        {
            if (zoneIds.Count == 0) return new();

            return await _db.Images
                .AsNoTracking()
                .Where(i => i.ZoneId != null && zoneIds.Contains(i.ZoneId.Value))
                .GroupBy(i => i.ZoneId!.Value)
                .Select(g => new { ZoneId = g.Key, Paths = g.Select(i => i.Path).ToList() })
                .ToDictionaryAsync(x => x.ZoneId, x => x.Paths);
        }

        private static ZoneResponseDto Map(Zone z, List<string> images) => new(
            z.Id, z.Title, z.Subtitle, z.Category, z.Price,
            z.Emoji, z.Tag, z.Popular,
            z.Cpu, z.Gpu, z.Ram, z.Monitor,
            z.Description, z.Features, images);
    }
}