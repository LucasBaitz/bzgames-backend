using BZGames.Application.Common.Extensions;
using BZGames.Application.DTOs.Games.BTS;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.Interfaces.Managers;
using BZGames.Application.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BZGames.API.Hubs
{
    [SignalRHub]
    [Authorize]
    public class BTSHub : Hub
    {
        private readonly static Dictionary<string, BTSPlayer> _connections = new Dictionary<string, BTSPlayer>();
        private static readonly IBTSGameManager _gameManager = new BTSManager();
        private readonly ILogger<BTSHub> _logger;

        public BTSHub(ILogger<BTSHub> logger)
        {
            _logger = logger;
        }
        private string IdentityName
        {
            get
            {
                var userName = Context.User?.GetUserName();
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Invalid token provided.");
                }
                return userName;
            }
        }

        public override async Task OnConnectedAsync()
        {
            BTSPlayer newPlayer = new(IdentityName, Context.ConnectionId);

            _connections.Add(Context.ConnectionId, newPlayer);

            _logger.LogInformation($"There are a total of: {_connections.Count} Active players");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId, out _);
            await Disconnect();
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"There are a total of: {_connections.Count} Active players");
        }

        public async Task CreateLobby(CreateLobby createLobby)
        {
            BTSPlayer? player = _connections[Context.ConnectionId];

            if (_gameManager.LobbyWithNameExists(createLobby.LobbyName))
            {
                await Clients.Caller.SendAsync("LobbyCreationFailed", "Lobby name already exists.");
                return;
            }


            BTSMatch match = _gameManager.CreateLobby(createLobby, player);

            _logger.LogInformation($"Match: {match.PlayerOne.UserName} {match.CurrentPlayer.UserName}");

            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);

            BTSMatchState matchState = new(player, new("", ""), player, new());

            await Clients.Caller.SendAsync("LobbyCreated", matchState);
        }


        public async Task JoinGame(JoinLobby joinDto)
        {
            BTSMatch? match = _gameManager.GetLobbyByName(joinDto.LobbyName);

            if (match is null)
            {
                await Clients.Caller.SendAsync("JoinGameFailed", "Lobby does not exist.");
                return;
            }

            BTSPlayer player = _connections[Context.ConnectionId];

            match.PlayerTwo = player;
            match.Players.Add(player);


            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);


            BTSMatchState matchState = new(match.PlayerOne, match.PlayerTwo, match.CurrentPlayer, new());

            await Clients.Group(match.Name).SendAsync("PlayerJoined", matchState);
            await Clients.Caller.SendAsync("JoinGameSuccess", matchState);
        }

        public async Task BuildBoard(BTSBoard board)
        {
            BTSPlayer player = _connections[Context.ConnectionId];
            BTSMatch? match = _gameManager.GetUserMatch(Context.ConnectionId);

            if (match is null)
            {
                _logger.LogError("No match found.");
            }


            if (!_gameManager.IsValidBoard(board))
            {
                await Clients.Caller.SendAsync("InvalidBoard", "Invalid Board");
                return;
            }

            player.Board = board;
            await Clients.Caller.SendAsync("BoardSet");


            bool allPlayersAreReady = match.Players.All(p => _gameManager.IsValidBoard(p.Board));

            if (allPlayersAreReady)
            {
                foreach (var p in match.Players)
                {
                    BTSPlayer otherPlayer = match.Players.First(x => x != p);
                    BTSMatchState matchState = new(p, otherPlayer, match.CurrentPlayer, otherPlayer.Board);
                    await Clients.Client(p.ConnectionId).SendAsync("StartMatch", matchState);
                }
            }
        }

        public async Task Disconnect()
        {
            var match = _gameManager.GetUserMatch(Context.ConnectionId);

            if (match is null) return;

            await Clients.OthersInGroup(match.Name).SendAsync("FF", "You won!! Your Opponent left!");

            foreach (BTSPlayer player in match.Players)
            {
                _logger.LogInformation($"Removing {player.UserName} from {match.Name} group");
                await Groups.RemoveFromGroupAsync(player.ConnectionId, match.Name);
            }

            match.Players.ForEach(_gameManager.ResetPlayer);

            _gameManager.DeleteMatch(match);
        }

        public async Task PlaceMove(int row, int col)
        {
            BTSPlayer player = _connections[Context.ConnectionId];

            BTSMatch? match = _gameManager.GetUserMatch(player.ConnectionId);

            if (match is null)
            {
                _logger.LogError("User match not found.");
                return;
            }

            if (match.CurrentPlayer != player)
            {
                _logger.LogError($"{player.UserName} tried to place a move but it is not their turn.");
                return;
            }

            BTSPlayer? opponent = match.Players.FirstOrDefault(p => p.UserName != player.UserName);

            if (opponent is null)
            {
                _logger.LogError("No opponent set.");
                return;
            }

            var moveHit = opponent.Board.PlaceMove(row, col);

            if (!moveHit)
            {
                match.SwapTurn();
            }

            foreach (var p in match.Players)
            {
                BTSPlayer otherPlayer = match.Players.First(x => x != p);
                BTSMatchState matchState = new(p, otherPlayer, match.CurrentPlayer, otherPlayer.Board);
                await Clients.Client(p.ConnectionId).SendAsync("MovePlaced", matchState);
            }

            if (!opponent.Board.HasShips())
            {
                await Clients.Group(match.Name).SendAsync("GameOver");

                foreach (var p in match.Players)
                {
                    p.Board = new BTSBoard();
                    await Groups.RemoveFromGroupAsync(p.ConnectionId, match.Name);
                }

                match.Players.Clear();
                _gameManager.DeleteMatch(match);
            }
        }
    }
}
