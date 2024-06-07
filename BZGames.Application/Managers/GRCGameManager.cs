using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.GRC;
using BZGames.Application.Interfaces.Managers;

namespace BZGames.Application.Managers
{
    public class GRCGameManager : IGRCGameManager
    {
        public Dictionary<Guid, GRCMatch> Lobbies { get; set; }

        public GRCGameManager()
        {
            Lobbies = new Dictionary<Guid, GRCMatch>();
        }


        public GRCMatch CreateLobby(CreateLobby createDto, GRCPlayer creator)
        {
            Guid matchId = Guid.NewGuid();
            GRCMatch newMatch = new()
            {
                Id = matchId,
                Name = createDto.LobbyName,
                CurrentPlayer = creator,
                Players = new() { { creator } },
                Words = createDto.Words
            };

            return newMatch;
        }
        public void DeleteMatch(GRCMatch match)
        {
            var matchEntry = Lobbies.Values.FirstOrDefault(m => m.Id == match.Id);

            bool result = Lobbies.Remove(matchEntry.Id);
        }

        public void AddPlayerToLobby(GRCMatch match, GRCPlayer player)
        {
            match.Players.Add(player);
        }

        public GRCMatch? GetLobbyByName(string lobbyName)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Name == lobbyName);
        }

        public GRCMatch? GetUserMatch(string connectionId)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Players.Any(p => p.ConnectionId == connectionId));
        }

        public bool LobbyWithNameExists(string lobbyName)
        {
            return Lobbies.Values.Any(l => l.Name.Equals(lobbyName));
        }

        public void ResetPlayer(GRCPlayer player)
        {
            player.Score = 0;
        }
    }
}
