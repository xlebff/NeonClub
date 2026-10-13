using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonClub.Api.Data;
using NeonClub.Api.Services;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    public class UploadsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly FileStorageService _files;

        public UploadsController(AppDbContext db, FileStorageService files)
        {
            _db = db;
            _files = files;
        }

        // POST /api/uploads/zone/{zoneId}   (multipart/form-data, поле "image")
        [HttpPost("zone/{zoneId:guid}")]
        [Authorize(Roles = "admin")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadZoneImage(Guid zoneId, IFormFile image)
        {
            var zone = await _db.Zones.FindAsync(zoneId);
            if (zone is null) return NotFound(new { error = "Зона не найдена" });

            var (ok, error, path, safeName) = await _files.SaveAsync(image);
            if (!ok) return BadRequest(new { error });

            var img = new Image
            {
                OriginalName = image.FileName,
                Path = path,
                MimeType = image.ContentType,
                SizeBytes = image.Length,
                ZoneId = zone.Id
            };

            _db.Images.Add(img);
            await _db.SaveChangesAsync();

            return StatusCode(201, new { id = img.Id, path = img.Path });
        }

        // GET /api/uploads   — список всех картинок
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _db.Images
                .AsNoTracking()
                .Include(i => i.Zone)
                .OrderByDescending(i => i.UploadedAt)
                .Select(i => new
                {
                    i.Id,
                    i.OriginalName,
                    i.Path,
                    i.MimeType,
                    i.SizeBytes,
                    i.UploadedAt,
                    ZoneId = i.ZoneId,
                    ZoneTitle = i.Zone != null ? i.Zone.Title : null
                })
                .ToListAsync();

            return Ok(images);
        }

        // DELETE /api/uploads/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var img = await _db.Images.FindAsync(id);
            if (img is null) return NotFound();

            _files.Delete(img.Path);       // снести файл с диска
            _db.Images.Remove(img);        // и строку из БД
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}