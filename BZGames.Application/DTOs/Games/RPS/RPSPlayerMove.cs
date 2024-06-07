namespace BZGames.Application.DTOs.Games.RPS
{
    public class RPSPlayerMove
    {
        public string UserName { get; set; } = string.Empty;
        public RPS LockedMove { get; set; } = RPS.None;
    }
}
