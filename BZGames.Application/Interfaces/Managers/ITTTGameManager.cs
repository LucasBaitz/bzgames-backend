using BZGames.Application.DTOs.Games.C4;
using BZGames.Application.DTOs.Games.TTT;

namespace BZGames.Application.Interfaces.Managers
{
    public interface ITTTGameManager : IGameManager<TTTMatch, TTTPlayer>
    {
        void AddPlayerToLobby(TTTMatch match, TTTPlayer player, TTTPiece piece);
        bool PlacePiece(TTTMatch match, TTTPlayer player, TTTMove postion);
        TTTPiece? CheckWinner(TTTMatch match);
    }
}
