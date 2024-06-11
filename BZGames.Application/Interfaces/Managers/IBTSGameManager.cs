using BZGames.Application.DTOs.Games.BTS;

namespace BZGames.Application.Interfaces.Managers
{
    public interface IBTSGameManager : IGameManager<BTSMatch, BTSPlayer>
    {
        bool IsValidBoard(BTSBoard board);
    }
}
