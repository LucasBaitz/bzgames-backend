using BZGames.Application.DTOs.Games.RPS;

namespace BZGames.Application.DTOs.RPS
{
    public class RPSMatch
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<RPSPlayer> Players { get; set; } = null!;

    }
}
