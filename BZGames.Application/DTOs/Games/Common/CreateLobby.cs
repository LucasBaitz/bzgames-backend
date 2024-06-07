namespace BZGames.Application.DTOs.Games.Common
{
    public record CreateLobby(string LobbyName, string Password, List<string> Words) { }
}
