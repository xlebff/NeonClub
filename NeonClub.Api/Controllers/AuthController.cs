using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonClub.Api.Data;
using NeonClub.Api.Services;
using NeonClub.Shared.Constracts;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AppDbContext db, JwtService jwt) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly JwtService _jwt = jwt;

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var (ok, msg) = dto.Validate();
            if (!ok) return BadRequest(new { error = msg.Trim() });

            var email = dto.Email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return Conflict(new { error = "Email уже используется" });

            var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "user")
                ?? throw new InvalidOperationException("Роль 'user' не найдена. Запусти сидер.");

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = userRole.Id
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            user.Role = userRole;

            var token = _jwt.GenerateToken(user);
            var userDto = new UserResponseDto(user.Id, user.Name, user.Email, user.Role.Name, user.CreatedAt);

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new AuthResponseDto(token, userDto));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var (ok, msg) = dto.Validate();
            if (!ok) return BadRequest(new { error = msg.Trim() });

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { error = "Неверный email или пароль" });

            var token = _jwt.GenerateToken(user);
            var userDto = new UserResponseDto(user.Id, user.Name, user.Email, user.Role.Name, user.CreatedAt);

            return Ok(new AuthResponseDto(token, userDto));
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            return user is null
                ? NotFound()
                : Ok(new UserResponseDto(user.Id, user.Name, user.Email, user.Role.Name, user.CreatedAt));
        }
    }
}
