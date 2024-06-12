using BZGames.Application.Common.Extensions;
using BZGames.Application.DTOs.Games.C4;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.TTT;
using BZGames.Application.Interfaces.Managers;
using BZGames.Application.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BZGames.API.Hubs
{
    [SignalRHub]
    [Authorize]
    public class TTTHub : Hub
    {
        private readonly static Dictionary<string, TTTPlayer> _connections = new Dictionary<string, TTTPlayer>();
        private static readonly ITTTGameManager _gameManager = new TTTManager();
        private readonly ILogger<TTTHub> _logger;

        public TTTHub(ILogger<TTTHub> logger)
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
            TTTPlayer newPlayer = new(IdentityName, Context.ConnectionId);

            _connections.Add(Context.ConnectionId, newPlayer);

            _logger.LogInformation($"There are a total of: {_connections.Count} Active players TTT");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId, out _);
            await Disconnect();
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"There are a total of: {_connections.Count} Active players on TTT");
        }

        public async Task CreateLobby(CreateLobby createLobby)
        {
            TTTPlayer? player = _connections[Context.ConnectionId];

            if (_gameManager.LobbyWithNameExists(createLobby.LobbyName))
            {
                await Clients.Caller.SendAsync("LobbyCreationFailed", "Lobby name already exists.");
                return;
            }
            
            TTTMatch match = _gameManager.CreateLobby(createLobby, player);

            _logger.LogInformation($"Match: {match.XPlayer.UserName} {match.CurrentPlayer.UserName} {match.CurrentPlayer.Piece}");


            await Groups.AddToGroupAsync(Context.ConnectionId, match.Name);
            await Clients.Caller.SendAsync("LobbyCreated", match.Name);
        }


        public async Task JoinGame(JoinLobby joinDto)
        {
            TTTMatch? lobby = _gameManager.GetLobbyByName(joinDto.LobbyName);

            if (lobby is null)
            {
                await Clients.Caller.SendAsync("JoinGameFailed", "Lobby does not exist.");
                return;
            }

            TTTPlayer player = _connections[Context.ConnectionId];

            lobby.SetOPlayer(player);

            TTTMatchState matchInitialState = new(lobby.XPlayer, lobby.OPlayer, lobby.Board, lobby.CurrentPlayer);

            await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Name);
            await Clients.OthersInGroup(lobby.Name).SendAsync("PlayerJoined", player.UserName);
            await Clients.Group(lobby.Name).SendAsync("StartMatch", matchInitialState); ;
        }

        public async Task PlaceMove(TTTMove move)
        {
            _logger.LogInformation($"X: {move.Row}, Y: {move.Column}");
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

            if (!match.Board.CanPlacePiece(move))
            {
                await Clients.Caller.SendAsync("InvalidMove", "Cannot place a piece in the selected column.");
                return;
            }

            move.Piece = user.Piece;
            match.Board.PlacePiece(move);

            _logger.LogInformation($"Current player is {match.CurrentPlayer.UserName}, {match.CurrentPlayer.Piece}");

            match.SwitchTurn();

            var matchState = new TTTMatchState(match.XPlayer, match.OPlayer, match.Board, match.CurrentPlayer);

            await Clients.Group(match.Name).SendAsync("MovePlaced", matchState);

            var winningPiece = _gameManager.CheckWinner(match);

            bool tie = matchState.Board.IsFull();

            if (tie)
            {
                await Clients.Group(match.Name).SendAsync("Tie");

                foreach (TTTPlayer player in match.Players)
                {
                    _logger.LogInformation($"Removing {player.UserName} from {match.Name} group");
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, match.Name);
                }

                _gameManager.DeleteMatch(match);
            }

            if (winningPiece is not null)
            {
                _logger.LogInformation($"Winner is {winningPiece}");
                TTTPlayer winner = match.XPlayer.Piece == winningPiece ? match.XPlayer : match.OPlayer;

                matchState.Winner = winner;

                await Clients.Group(match.Name).SendAsync("GameOver", matchState);


                foreach (TTTPlayer player in match.Players)
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

            foreach (TTTPlayer player in match.Players)
            {
                _logger.LogInformation($"Removing {player.UserName} from {match.Name} group");
                await Groups.RemoveFromGroupAsync(player.ConnectionId, match.Name);
                _gameManager.ResetPlayer(player);
            }

            _gameManager.DeleteMatch(match);
        }
    }
}
