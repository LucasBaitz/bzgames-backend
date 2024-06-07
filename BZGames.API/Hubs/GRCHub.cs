using BZGames.Application.Common.Extensions;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.GRC;
using BZGames.Application.Interfaces.Managers;
using BZGames.Application.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BZGames.API.Hubs
{
    [SignalRHub]
    [Authorize]
    public class GRCHub : Hub
    {
        private readonly static Dictionary<string, GRCPlayer> _connections = new Dictionary<string, GRCPlayer>();
        private static readonly IGRCGameManager _gameManager = new GRCGameManager();
        private readonly ILogger<GRCHub> _logger;

        public GRCHub(ILogger<GRCHub> logger)
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
            GRCPlayer newPlayer = new(IdentityName, Context.ConnectionId);

            _connections.Add(Context.ConnectionId, newPlayer);

            _logger.LogInformation($"There are a total of: {_connections.Count} Active players (GRC Hub)");

            await base.OnConnectedAsync();
        }

        public async Task CreateLobby(CreateLobby createLobby)
        {
            GRCPlayer? player = _connections[Context.ConnectionId];

            if (_gameManager.LobbyWithNameExists(createLobby.LobbyName))
            {
                await Clients.Caller.SendAsync("LobbyCreationFailed", "Lobby name already exists.");
                return;
            }

            GRCMatch match = _gameManager.CreateLobby(createLobby, player);
            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);
            await Clients.Caller.SendAsync("LobbyCreated", match.Name);
        }

        public async Task JoinGame(JoinLobby joinDto)
        {

            GRCMatch? lobby = _gameManager.GetLobbyByName(joinDto.LobbyName);

            if (lobby is null)
            {
                await Clients.Caller.SendAsync("JoinGameFailed", "Lobby does not exist.");
                return;
            }

            if (lobby.Players.Count >= 8)
            {
                await Clients.Caller.SendAsync("LobbyFull", "Unable to join a ongoing match.");
                return;
            }


            GRCPlayer player = _connections[Context.ConnectionId];

            _gameManager.AddPlayerToLobby(lobby, player);

            await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Name);
            await Clients.OthersInGroup(lobby.Name).SendAsync("PlayerJoined", player.UserName);

            if (lobby.Players.Count >= 3)
            {
                await Clients.Group(lobby.Name).SendAsync("StartMatch", lobby.Players);
            }
        }

        public async Task DrawOnCanvas(GRCDrawngLine line)
        {
            GRCMatch? match = _gameManager.GetUserMatch(Context.ConnectionId);

            if (match is null)
            {
                _logger.LogWarning("Could not find the user match");
                return;
            }

            match.CurrentDrawing.Add(line);
    
            await Clients.Others.SendAsync("Drawing", line);
        }

        public async Task TakeGuess(string guess)
        {
            GRCMatch? match = _gameManager.GetUserMatch(Context.ConnectionId);
            GRCPlayer player = _connections[Context.ConnectionId];

            if (guess.ToUpper().Equals(match.CurrentWord.ToUpper()))
            {
                player.Score += match.CurrentMaxPoints;
                match.DescreseMaxPoints();

                await Clients.Group(match.Name).SendAsync("PlayerScored", "Player scored!");
            }

            await Clients.Caller.SendAsync("WrongGuess", "Wrong guess, keep trying");
        }
    }
}
