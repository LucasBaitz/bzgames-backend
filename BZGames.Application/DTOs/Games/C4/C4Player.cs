using BZGames.Application.DTOs.Games.Common;

namespace BZGames.Application.DTOs.Games.C4
{
    public sealed class C4Player : Player
    {
        public C4Player(string userName, string connectionId) : base(userName, connectionId) { }
        public C4Piece Piece { get; set; } = C4Piece.Empty;
    }
}
