using BZGames.Application.DTOs.Games.C4;

namespace BZGames.Application.Interfaces.Managers
{
    public interface IC4GameManager : IGameManager<C4Match, C4Player>
    {
        void AddPlayerToLobby(C4Match match, C4Player player, C4Piece piece);
        bool PlacePiece(C4Match match, C4Player player, int column);
        C4Piece? CheckWinner(C4Match match);

    }
}
