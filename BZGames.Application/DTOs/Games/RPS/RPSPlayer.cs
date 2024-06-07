using BZGames.Application.DTOs.Games.Common;

namespace BZGames.Application.DTOs.Games.RPS
{
    public sealed class RPSPlayer : Player
    {        
        public int Score { get; set; } = 0;
        public RPS LockedMove { get; set; } = RPS.None;
        public RPSPlayer(string userName, string connectionId) : base(userName, connectionId) { }
    }
}
