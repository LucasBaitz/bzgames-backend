using BZGames.Application.DTOs.Games.BTS;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.Interfaces.Managers;

namespace BZGames.Application.Managers
{
    public class BTSManager : IBTSGameManager
    {
        public Dictionary<Guid, BTSMatch> Lobbies { get; set; }

        public BTSManager()
        {
            Lobbies = new();
        }

        public BTSMatch CreateLobby(CreateLobby createDto, BTSPlayer creator)
        {

            Console.WriteLine(creator.UserName);

            var lobbyId = Guid.NewGuid();
            var newMatch = new BTSMatch()
            {
                Id = lobbyId,
                Name = createDto.LobbyName,
                PlayerOne = creator,
                CurrentPlayer = creator,
                Players = new() { creator }
            };

            newMatch.CurrentPlayer = creator;

            Console.WriteLine(newMatch.PlayerOne.UserName);

            Lobbies.Add(lobbyId, newMatch);

            return newMatch;
        }

        public void DeleteMatch(BTSMatch match)
        {
            var matchEntry = Lobbies.Values.FirstOrDefault(m => m.Id == match.Id);

            matchEntry.Players.Clear();

            bool result = Lobbies.Remove(matchEntry.Id);
        }

        public BTSMatch? GetLobbyByName(string lobbyName)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Name.Equals(lobbyName));
        }

        public BTSMatch? GetUserMatch(string connectionId)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Players.FirstOrDefault(p => p.ConnectionId == connectionId) != null);
        }

        public bool LobbyWithNameExists(string lobbyName)
        {
            return Lobbies.Any(l => l.Value.Name == lobbyName);
        }

        public bool IsValidBoard(BTSBoard board)
        {
            int expectedShips = 17;
            int shipCounter = 0;

            for (int i = 0; i < board.Layout.Count; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (board.Layout[i][j] == BTSPiece.Ship)
                    {
                        shipCounter++;
                    }
                }
            } 

            return shipCounter == expectedShips;
        }

        public void ResetPlayer(BTSPlayer player)
        {
            player.Board = new();
        }
    }
}
