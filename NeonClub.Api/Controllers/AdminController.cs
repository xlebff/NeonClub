using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeonClub.Api.Data;
using NeonClub.Shared.Constracts;
using Microsoft.EntityFrameworkCore;

namespace NeonClub.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "admin")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) => _db = db;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    Role = u.Role.Name,
                    u.CreatedAt,
                    BookingsCount = u.Bookings.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null) return NotFound();
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _db.Bookings
                .AsNoTracking()
                .Include(b => b.Zone)
                .Include(b => b.User)
                .Include(b => b.Status)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    ZoneTitle = b.Zone.Title,
                    ZoneEmoji = b.Zone.Emoji,
                    UserName = b.User.Name,
                    UserEmail = b.User.Email,
                    b.Hours,
                    b.Total,
                    Status = b.Status.Name,
                    b.CreatedAt
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPatch("bookings/{id:guid}/status")]
        public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] UpdateBookingStatusDto dto)
        {
            var status = await _db.Statuses.FirstOrDefaultAsync(s => s.Name == dto.Status);
            if (status is null) return BadRequest(new { error = "Недопустимый статус" });

            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            booking.StatusId = status.Id;
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _db.Statuses
                .AsNoTracking()
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();
            return Ok(statuses);
        }

        [HttpGet("zones")]
        public async Task<IActionResult> GetZones()
        {
            var zones = await _db.Zones
                .AsNoTracking()
                .OrderBy(z => z.Title)
                .Select(z => new { z.Id, z.Title })
                .ToListAsync();
            return Ok(zones);
        }
    }
}
