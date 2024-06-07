using BZGames.Application.DTOs.Games.Common;

namespace BZGames.Application.DTOs.Games.GRC
{
    public class GRCPlayer : Player
    {
        public int Score { get; set; } = 0;
        public GRCPlayer(string userName, string connectionId) : base(userName, connectionId) { }
    }
}
