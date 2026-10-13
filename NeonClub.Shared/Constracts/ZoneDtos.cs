namespace NeonClub.Shared.Constracts
{
    public record ZoneResponseDto(
        Guid Id, string Title, string Subtitle, string Category, decimal Price,
        string Emoji, string? Tag, bool Popular,
        string Cpu, string Gpu, string Ram, string Monitor,
        string Description, List<string> Features,
        List<string> Images);
}
