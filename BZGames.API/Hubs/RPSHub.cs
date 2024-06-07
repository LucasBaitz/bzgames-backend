using BZGames.API.Managers;
using BZGames.Application.Common.Extensions;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.RPS;
using BZGames.Application.DTOs.RPS;
using BZGames.Application.Interfaces.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BZGames.API.Hubs
{
    [SignalRHub]
    [Authorize]
    public class RPSHub : Hub
    {
        private readonly static Dictionary<string, RPSPlayer> _connections = new Dictionary<string, RPSPlayer>();
        private static readonly IRPSGameManager _gameManager = new RPSManager();
        private readonly ILogger<RPSHub> _logger;

        public RPSHub(ILogger<RPSHub> logger)
        {
            _logger = logger;
        }

        private string IdentityName
        {
            get
            {
                var userName = Context.User!.GetUserName();
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Invalid token was provided");
                }

                return userName;
            }
        }

        public override async Task OnConnectedAsync()
        {
            RPSPlayer newPlayer = new(IdentityName, Context.ConnectionId);
            _connections.Add(Context.ConnectionId, newPlayer);

            await base.OnConnectedAsync();
        }

        public async Task CreateLobby(CreateLobby createLobby)
        {
            RPSPlayer? player = _connections[Context.ConnectionId];

            if (_gameManager.LobbyWithNameExists(createLobby.LobbyName))
            {
                await Clients.Caller.SendAsync("LobbyCreationFailed", "Lobby name already exists.");
                return;
            }

            RPSMatch match = _gameManager.CreateLobby(createLobby, player);
            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);
            await Clients.Caller.SendAsync("LobbyCreated", match.Name);
        }


        public async Task JoinGame(JoinLobby joinDto)
        {

            RPSMatch? lobby = _gameManager.GetLobbyByName(joinDto.LobbyName);

            if (lobby is null)
            {
                await Clients.Caller.SendAsync("JoinGameFailed", "Lobby does not exist.");
                return;
            }

            if (lobby.Players.Count >= 2)
            {
                await Clients.Caller.SendAsync("LobbyFull", "Unable to join a ongoing match.");
                return;
            }


            RPSPlayer player = _connections[Context.ConnectionId];

            _gameManager.AddPlayerToLobby(lobby.Id, player);
           
            await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Name);
            await Clients.OthersInGroup(lobby.Name).SendAsync("PlayerJoined", player.UserName);
            await Clients.Group(lobby.Name).SendAsync("StartMatch", lobby.Players);;
        }

        public async Task LockMove(RPS move)
        {
            var match = _gameManager.GetUserMatch(Context.ConnectionId);         

            if (match is null)
            {
                _logger.LogWarning("Match not found!");
                return;
            }

            var player = match.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            player.LockedMove = move;

            if (match.Players.All(p => p.LockedMove != RPS.None))
            {
                RPSRoundResult roundResult = _gameManager.CalculateRoundWinner(match);

                if (roundResult.Winner is null)
                {
                    await Clients.Group(match.Name).SendAsync("Tie", roundResult);
                    match.Players.ForEach(_gameManager.ResetPlayerMove);
                    return;
                }

                var winner = roundResult.Players.FirstOrDefault(p => p.Score == 3);

                if (winner is not null)
                {
                    await Clients.Group(match.Name).SendAsync("GameOver", winner.UserName);

                    foreach (var playerUser in match.Players)
                    {
                        _logger.LogInformation($"Removing {playerUser.UserName} from {match.Name} group");
                        await Groups.RemoveFromGroupAsync(playerUser.ConnectionId, match.Name);
                    }

                    match.Players.ForEach(_gameManager.ResetPlayerMove);
                    _gameManager.DeleteMatch(match);
                    _logger.LogInformation($"GameOver reached! Total of active lobbies is: {_gameManager.Lobbies.Count}");
                    return;
                }
                await Clients.Group(match.Name).SendAsync("PlayerScored", roundResult);

                match.Players.ForEach(_gameManager.ResetPlayerMove);
            }
        }

        public async Task MakeMove(RPSMove move)
        {
            if (move is null || move.RPS == RPS.None) return;

            var match = _gameManager.GetUserMatch(Context.ConnectionId);
            if (match is null) return;

            var reqPlayer = match.Players.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (reqPlayer is null) return;

            reqPlayer.LockedMove = move.RPS;

            var playerMove = new RPSPlayerMove()
            {
                LockedMove = reqPlayer.LockedMove,
                UserName = reqPlayer.UserName
            };

           await Clients.Group(match.Name).SendAsync("PlayerMadeMove", playerMove);
            _logger.LogInformation($"A {move.RPS} move was made! by {reqPlayer.UserName} at {match.Name} Lobby!");

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Disconnect();
            _connections.Remove(Context.ConnectionId);

        
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Disconnect()
        {
            var match = _gameManager.GetUserMatch(Context.ConnectionId);

            if (match is null) return;

            await Clients.OthersInGroup(match.Name).SendAsync("FF", "You won!! Your Opponent left!");
            match.Players.ForEach(_gameManager.ResetPlayer);
            _gameManager.DeleteMatch(match);
        }
    }
}
