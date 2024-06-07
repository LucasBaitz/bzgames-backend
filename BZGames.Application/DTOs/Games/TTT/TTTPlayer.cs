using BZGames.Application.DTOs.Games.Common;

namespace BZGames.Application.DTOs.Games.TTT
{
    public class TTTPlayer : Player
    {
        public TTTPiece Piece { get; set; } = TTTPiece.Empty;
        public TTTPlayer(string userName, string connectionId) : base(userName, connectionId) { }
    }
}
