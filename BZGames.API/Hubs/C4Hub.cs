using BZGames.Application.Common.Extensions;
using BZGames.Application.DTOs.Games.C4;
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
    public class C4Hub : Hub
    {
        private readonly static Dictionary<string, C4Player> _connections = new Dictionary<string, C4Player>();
        private static readonly IC4GameManager _gameManager = new C4Manager();
        private readonly ILogger<C4Hub> _logger;

        public C4Hub(ILogger<C4Hub> logger)
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
            C4Player newPlayer = new(IdentityName, Context.ConnectionId);

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
            C4Player? player = _connections[Context.ConnectionId];

            if (_gameManager.LobbyWithNameExists(createLobby.LobbyName))
            {
                await Clients.Caller.SendAsync("LobbyCreationFailed", "Lobby name already exists.");
                return;
            }

            C4Match match = _gameManager.CreateLobby(createLobby, player);

            _logger.LogInformation($"Match: {match.RedPlayer.UserName} {match.CurrentPlayer.UserName} {match.CurrentPlayer.Piece}");

            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);
            await Clients.Caller.SendAsync("LobbyCreated", match.Name);
        }


        public async Task JoinGame(JoinLobby joinDto)
        {
            C4Match? lobby = _gameManager.GetLobbyByName(joinDto.LobbyName);

            if (lobby is null)
            {
                await Clients.Caller.SendAsync("JoinGameFailed", "Lobby does not exist.");
                return;
            }

            C4Player player = _connections[Context.ConnectionId];

            lobby.SetYellowPlayer(player);

            C4MatchState matchInitialState = new(lobby.RedPlayer, lobby.YellowPlayer, lobby.Board, lobby.CurrentPlayer);

            await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Name);
            await Clients.OthersInGroup(lobby.Name).SendAsync("PlayerJoined", player.UserName);
            await Clients.Group(lobby.Name).SendAsync("StartMatch", matchInitialState); ;
        }

        public async Task PlaceMove(int column)
        {
            _logger.LogInformation($"{IdentityName} is placing a move");
            var match = _gameManager.Lobbies.Values.FirstOrDefault(l => l.Players.Any(p => p.UserName == IdentityName));

            if (match == null)
            {
                await Clients.Caller.SendAsync("MatchNotFound", "No match found for the user.");
                return;
            }

            var user = match.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

            if (user == null)
            {
                await Clients.Caller.SendAsync("PlayerNotFound", "Player information not found.");
                return;
            }

            if (!match.IsPlayerTurn(user))
            {
                await Clients.Caller.SendAsync("NotYourTurn", "Wait until your opponent makes a move.");
                return;
            }

            if (!match.Board.CanPlacePiece(new C4Move() { Column = column }))
            {
                await Clients.Caller.SendAsync("InvalidMove", "Cannot place a piece in the selected column.");
                return;
            }

            match.Board.PlacePiece(new C4Move() { Column = column, Piece = user.Piece });

            _logger.LogInformation($"Current player is {match.CurrentPlayer.UserName}, {match.CurrentPlayer.Piece}");

            match.SwitchTurn();
            
            var matchState = new C4MatchState(match.RedPlayer, match.YellowPlayer, match.Board, match.CurrentPlayer);

            await Clients.Group(match.Name).SendAsync("MovePlaced", matchState);

            var winningPiece = _gameManager.CheckWinner(match);


            if (winningPiece is not null)
            {
                C4Player winner = match.RedPlayer.Piece == winningPiece ? match.RedPlayer : match.YellowPlayer;

                matchState.Winner = winner;

                await Clients.Group(match.Name).SendAsync("GameOver", matchState);


                foreach (C4Player player in match.Players)
                {
                    _logger.LogInformation($"Removing {player.UserName} from {match.Name} group");
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, match.Name);
                }

                _gameManager.DeleteMatch(match);

                _logger.LogInformation($"The are a total of {_gameManager.Lobbies.Count} active lobbies");
                return;
            }
        }

        public async Task Disconnect()
        {
            var match = _gameManager.GetUserMatch(Context.ConnectionId);

            if (match is null) return;

            await Clients.OthersInGroup(match.Name).SendAsync("FF", "You won!! Your Opponent left!");

            foreach (C4Player player in match.Players)
            {
                _logger.LogInformation($"Removing {player.UserName} from {match.Name} group");
                await Groups.RemoveFromGroupAsync(player.ConnectionId, match.Name);
            }

            match.Players.ForEach(_gameManager.ResetPlayer);
            
            _gameManager.DeleteMatch(match);
        }
    }
}
