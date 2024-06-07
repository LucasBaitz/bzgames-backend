using BZGames.Application.DTOs.Games.Common;

namespace BZGames.Application.Interfaces.Managers
{
    public interface IGameManager<TGameMatch, TGamePlayer> where TGamePlayer : Player
    {
        Dictionary<Guid, TGameMatch> Lobbies { get; }
        TGameMatch CreateLobby(CreateLobby createDto, TGamePlayer creator);
        void DeleteMatch(TGameMatch match);
        TGameMatch? GetUserMatch(string connectionId);
        bool LobbyWithNameExists(string lobbyName);
        TGameMatch? GetLobbyByName(string lobbyName);
        void ResetPlayer(TGamePlayer player);
    }
}
