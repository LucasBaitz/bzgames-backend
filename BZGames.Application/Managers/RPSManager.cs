using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.RPS;
using BZGames.Application.DTOs.RPS;
using BZGames.Application.Interfaces.Managers;

namespace BZGames.API.Managers
{
    public sealed class RPSManager : IRPSGameManager
    {
        public Dictionary<Guid, RPSMatch> Lobbies { get; set; }

        public RPSManager()
        {
            Lobbies = new Dictionary<Guid, RPSMatch>();
        }
        public RPSMatch CreateLobby(CreateLobby createDto, RPSPlayer creator) 
        {
            Guid lobbyId = Guid.NewGuid();

            var newMatch = new RPSMatch()
            {
                Id = lobbyId,
                Name = createDto.LobbyName,
                Players = new List<RPSPlayer>() { creator }
            };

            Lobbies.Add(lobbyId, newMatch);

            return newMatch;
        }

        public void DeleteMatch(RPSMatch match)
        {
            var matchEntry = Lobbies.Values.FirstOrDefault(m => m.Id == match.Id);

            matchEntry.Players.Clear();

            bool result = Lobbies.Remove(matchEntry.Id);
        }

        public void AddPlayerToLobby(Guid lobbyId, RPSPlayer player)
        {

            Lobbies[lobbyId].Players.Add(player);
        }

        //public async Task CalculatePlayerRating(RPSUserMatchReport matchReport)
        //{
        //    var user = _userManager.Users.FirstOrDefault(u => u.NormalizedUserName == matchReport.UserName.ToUpper());

        //    if (matchReport.Won)
        //    {
        //        user.Rating += new Random().NextDouble() * (0.1 - 0.7);
        //    }
        //    else
        //    {
        //        user.Rating -= new Random().NextDouble() * (0.1 - 0.5);
        //    }

        //    await _userManager.UpdateAsync(user);
        //}

        public bool LobbyExists(Guid lobbyId)
        {
            return Lobbies.ContainsKey(lobbyId);
        }

        public bool LobbyWithNameExists(string name)
        {
            return Lobbies.Any(l => l.Value.Name == name);
        }

        public RPSMatch? GetLobbyByName(string lobbyName)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Name == lobbyName);
        }

        public RPSMatch? GetUserMatch(string userConnection)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Players.Any(u => u.ConnectionId == userConnection));
        }

        public RPSPlayer? GetPlayer(string userConnection)
        {
            return Lobbies.Values
                          .SelectMany(lobby => lobby.Players)
                          .FirstOrDefault(player => player.ConnectionId == userConnection);
        }

        public RPSRoundResult CalculateRoundWinner(RPSMatch match)
        {

            var playerOne = match.Players[0];
            var playerTwo = match.Players[1];

            var roundResult = new RPSRoundResult()
            {
                Players = new List<RPSPlayer>() { playerOne, playerTwo },
            };

            var moveOne = playerOne.LockedMove;
            var moveTwo = playerTwo.LockedMove;


            if (playerOne.LockedMove == playerTwo.LockedMove)
            {
                roundResult.Winner = null!;
                return roundResult;
            }
            if ((moveOne == RPS.Rock && moveTwo == RPS.Scissors) ||
                     (moveOne == RPS.Scissors && moveTwo == RPS.Paper) ||
                     (moveOne == RPS.Paper && moveTwo == RPS.Rock))
            {
                playerOne.Score++;
                roundResult.Winner = playerOne;
            }
            else
            {
                playerTwo.Score++;
                roundResult.Winner = playerTwo;                
            }


            return roundResult;
        }


        public void ResetPlayer(RPSPlayer player)
        {
            ResetPlayerMove(player);
            ResetPlayerScore(player);
        }

        public void ResetPlayerScore(RPSPlayer player)
        {
            player.Score = 0;
        }

        public void ResetPlayerMove(RPSPlayer player)
        {
            player.LockedMove = RPS.None;
        }
    }
}
