using BZGames.Application.DTOs.Games.C4;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.DTOs.Games.TTT;
using BZGames.Application.Interfaces.Managers;

namespace BZGames.Application.Managers
{
    public class TTTManager : ITTTGameManager
    {
        public Dictionary<Guid, TTTMatch> Lobbies { get; set; }

        public TTTManager()
        {
            Lobbies = new Dictionary<Guid, TTTMatch>();
        }

        public TTTMatch CreateLobby(CreateLobby createDto, TTTPlayer creator)
        {

            Console.WriteLine(creator.UserName);

            var lobbyId = Guid.NewGuid();
            var newMatch = new TTTMatch()
            {
                Id = lobbyId,
                Name = createDto.LobbyName,
                XPlayer = creator,
                CurrentPlayer = creator,
                Players = new() { creator }
            };

            newMatch.SetXPlayer(creator);
            newMatch.CurrentPlayer = creator;
            Lobbies.Add(lobbyId, newMatch);

            return newMatch;
        }

        public void DeleteMatch(TTTMatch match)
        {
            var matchEntry = Lobbies.Values.FirstOrDefault(m => m.Id == match.Id);

            matchEntry.Players.ForEach(p =>
            {
                p.Piece = TTTPiece.Empty;
            });

            matchEntry.Players.Clear();

            bool result = Lobbies.Remove(matchEntry.Id);
            Console.WriteLine($"Deleting {match.Name} was {result}");
        }

        public void AddPlayerToLobby(TTTMatch match, TTTPlayer player, TTTPiece piece)
        {
            if (piece == TTTPiece.X)
            {
                match.SetXPlayer(player);
            }
            else
            {
                match.SetOPlayer(player);
            }
        }

        public TTTMatch? GetUserMatch(string connectionId)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Players.FirstOrDefault(p => p.ConnectionId == connectionId) != null);
        }

        public bool PlacePiece(TTTMatch match, TTTPlayer player, TTTMove position)
        {
            if (!match.IsPlayerTurn(player) || !match.Board.CanPlacePiece(position))
            {
                return false;
            }

            var move = position;
            match.Board.PlacePiece(move);
            return true;
        }

        public TTTPiece? CheckWinner(TTTMatch match)
        {
            var board = match.Board.Layout;
            int rows = match.Board.Rows;
            int cols = match.Board.Columns;

            // Horizontal check
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (board[row][col] != TTTPiece.Empty &&
                        board[row][col] == board[row][col + 1] &&
                        board[row][col] == board[row][col + 2])
                    {
                        return board[row][col];
                    }
                }
            }

            // Vertical check
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 2; row++)
                {
                    if (board[row][col] != TTTPiece.Empty &&
                        board[row][col] == board[row + 1][col] &&
                        board[row][col] == board[row + 2][col])
                    {
                        return board[row][col];
                    }
                }
            }

            // Top-left to bottom-right check
            for (int row = 0; row < rows - 2; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (board[row][col] != TTTPiece.Empty &&
                        board[row][col] == board[row + 1][col + 1] &&
                        board[row][col] == board[row + 2][col + 2])
                    {
                        return board[row][col];
                    }
                }
            }

            // Top-right to bottom-left check
            for (int row = 0; row < rows - 2; row++)
            {
                for (int col = 2; col < cols; col++)
                {
                    if (board[row][col] != TTTPiece.Empty &&
                        board[row][col] == board[row + 1][col - 1] &&
                        board[row][col] == board[row + 2][col - 2])
                    {
                        return board[row][col];
                    }
                }
            }

            return null;
        }

        public void ResetPlayer(TTTPlayer player)
        {
            player.Piece = TTTPiece.Empty;
        }


        public bool LobbyWithNameExists(string lobbyName)
        {
            return Lobbies.Any(l => l.Value.Name.Equals(lobbyName));
        }

        public TTTMatch? GetLobbyByName(string lobbyName)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Name == lobbyName);
        }
    }
}
