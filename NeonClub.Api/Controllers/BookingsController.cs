using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonClub.Api.Data;
using NeonClub.Shared.Constracts;
using NeonClub.Shared.Extensions;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BookingsController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto)
        {
            var (ok, msg) = dto.Validate();
            if (!ok) return BadRequest(new { error = msg.Trim() });

            var userId = User.GetUserId();
            var user = await _db.Users.FindAsync(userId);
            if (user is null) return Unauthorized(new { error = "Пользователь не найден" });

            var zone = await _db.Zones.FindAsync(dto.ZoneId);
            if (zone is null) return NotFound(new { error = "Зона не найдена" });

            var newStatus = await _db.Statuses.FirstAsync(s => s.Name == "новый");

            var booking = new Booking
            {
                UserId = user.Id,
                ZoneId = zone.Id,
                Hours = dto.Hours,
                Total = zone.Price * dto.Hours,
                StatusId = newStatus.Id
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            booking.Status = newStatus;
            booking.Zone = zone;

            return StatusCode(201, Map(booking, user, zone));
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> My()
        {
            var userId = User.GetUserId();

            var bookings = await _db.Bookings
                .AsNoTracking()
                .Include(b => b.Zone)
                .Include(b => b.User)
                .Include(b => b.Status)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(bookings.Select(b => Map(b, b.User, b.Zone)));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> All()
        {
            var bookings = await _db.Bookings
                .AsNoTracking()
                .Include(b => b.Zone)
                .Include(b => b.User)
                .Include(b => b.Status)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(bookings.Select(b => Map(b, b.User, b.Zone)));
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusDto dto)
        {
            var (ok, msg) = dto.Validate();
            if (!ok) return BadRequest(new { error = msg.Trim() });

            var status = await _db.Statuses.FirstOrDefaultAsync(s => s.Name == dto.Status);
            if (status is null) return BadRequest(new { error = "Недопустимый статус" });

            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            booking.StatusId = status.Id;
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();

            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return NotFound();
            if (booking.UserId != userId) return Forbid();

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        private static BookingResponseDto Map(Booking b, User u, Zone z) => new(
            b.Id, z.Id, z.Title, z.Emoji,
            b.Hours, b.Total, b.Status?.Name ?? "", b.CreatedAt,
            u.Id, u.Name, u.Email);
    }
}