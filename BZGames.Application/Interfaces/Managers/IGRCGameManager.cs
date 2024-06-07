using BZGames.Application.DTOs.Games.GRC;

namespace BZGames.Application.Interfaces.Managers
{
    public interface IGRCGameManager : IGameManager<GRCMatch, GRCPlayer>
    {
        void AddPlayerToLobby(GRCMatch match, GRCPlayer player);
    }
}
