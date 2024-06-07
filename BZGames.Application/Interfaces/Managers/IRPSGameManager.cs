using BZGames.Application.DTOs.Games.RPS;
using BZGames.Application.DTOs.RPS;

namespace BZGames.Application.Interfaces.Managers
{
    public interface IRPSGameManager : IGameManager<RPSMatch, RPSPlayer>
    {
        void AddPlayerToLobby(Guid lobbyId, RPSPlayer player);
        RPSRoundResult CalculateRoundWinner(RPSMatch match);
        void ResetPlayerMove(RPSPlayer player);
        void ResetPlayerScore(RPSPlayer player);
        void ResetPlayer(RPSPlayer player);
    }
}
