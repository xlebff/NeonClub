using neon_club_api.Extensions;
using neon_club_api.Models.Storage;
using NeonClub.Shared.Constracts.Interfaces;

namespace NeonClub.Shared.Constracts
{
    public record CreateBookingDto(Guid ZoneId, int Hours) : IDTO
    {
        public (bool IsValid, string Message) Validate()
        {
            bool ok = true;
            string msg = string.Empty;

            if (ZoneId == Guid.Empty)
            {
                ok = false;
                msg += "Не указана зона.\n";
            }

            Hours.IsInRangeValidation(ConstsStorage.MinBook, ConstsStorage.MaxBook, ref ok, ref msg);

            return (ok, msg);
        }
    }

    public record UpdateBookingStatusDto(string Status) : IDTO
    {
        public (bool IsValid, string Message) Validate()
        {
            bool ok = true;
            string msg = string.Empty;

            Status.NullOrEmptyValidation(ref ok, ref msg);

            return (ok, msg);
        }
    }

    public record BookingResponseDto(
        Guid Id, Guid ZoneId, string ZoneTitle, string ZoneEmoji,
        int Hours, decimal Total, string Status, DateTime CreatedAt,
        Guid UserId, string UserName, string UserEmail);
}
