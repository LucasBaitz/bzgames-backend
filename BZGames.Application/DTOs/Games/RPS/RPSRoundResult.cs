namespace BZGames.Application.DTOs.Games.RPS
{
    public class RPSRoundResult
    {
        public List<RPSPlayer> Players { get; set; } = null!;
        public RPSPlayer Winner { get; set; } = null!;

    }
}
