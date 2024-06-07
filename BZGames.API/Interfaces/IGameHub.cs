using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.RPS;

namespace BZGames.API.Interfaces
{
    public interface IGameHub
    {
        Task CreateLobby(string username, CreateLobby lobbyDto);
        Task UpdateGame();

        Task<string> LobbyCreationFailed(string message);
        Task<string> LobbyCreated(string message);
        Task<string> JoinGameFailed(string message);
        Task<string> PlayerJoined(string message);  
        Task<string> PlayerLeft(string message);
        Task<string> JoinGameSuccess(string message);
        Task<RPSPlayerMove> PlayerMadeMove(RPSPlayerMove playerMove);

    }
}
