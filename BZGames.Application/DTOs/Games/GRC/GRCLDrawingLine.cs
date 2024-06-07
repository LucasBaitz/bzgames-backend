namespace BZGames.Application.DTOs.Games.GRC
{
    public record GRCDrawngLine
    {
        public float PrevX { get; init; }
        public float PrevY { get; init; }
        public float CurrentX { get; init; }
        public float CurrentY { get; init; }
        public string Color { get; init; } = string.Empty;
        public int BrushSize { get; init; }
    }
}
