using neon_club_api.Extensions;
using NeonClub.Shared.Constracts.Interfaces;

namespace NeonClub.Shared.Constracts
{
    public record RegisterDto(string Name, string Email, string Password) : IDTO
    {
        public (bool IsValid, string Message) Validate()
        {
            bool ok = true;
            string msg = string.Empty;

            Name.NullOrEmptyValidation(ref ok, ref msg);
            Email.NullOrEmptyValidation(ref ok, ref msg);
            Password.NullOrEmptyValidation(ref ok, ref msg);

            if (!string.IsNullOrEmpty(Password) && Password.Length < 6)
            {
                ok = false;
                msg += "Пароль должен содержать минимум 6 символов.\n";
            }

            if (!string.IsNullOrEmpty(Email) && !Email.Contains('@'))
            {
                ok = false;
                msg += "Email указан некорректно.\n";
            }

            return (ok, msg);
        }
    }

    public record LoginDto(string Email, string Password) : IDTO
    {
        public (bool IsValid, string Message) Validate()
        {
            bool ok = true;
            string msg = string.Empty;

            Email.NullOrEmptyValidation(ref ok, ref msg);
            Password.NullOrEmptyValidation(ref ok, ref msg);

            return (ok, msg);
        }
    }

    public record UserResponseDto(Guid Id, string Name, string Email, string Role, DateTime CreatedAt);

    public record AuthResponseDto(string Token, UserResponseDto User);
}
