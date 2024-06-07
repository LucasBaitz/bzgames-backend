using BZGames.Application.DTOs.Games.C4;
using BZGames.Application.DTOs.Games.Common;
using BZGames.Application.Interfaces.Managers;

namespace BZGames.Application.Managers
{
    public sealed class C4Manager : IC4GameManager
    {
        public Dictionary<Guid, C4Match> Lobbies { get; set; }

        public C4Manager()
        {
            Lobbies = new Dictionary<Guid, C4Match>();
        }

        public C4Match CreateLobby(CreateLobby createDto, C4Player creator)
        {

            Console.WriteLine(creator.UserName);

            var lobbyId = Guid.NewGuid();
            var newMatch = new C4Match()
            {
                Id = lobbyId,
                Name = createDto.LobbyName,
                RedPlayer = creator,
                CurrentPlayer = creator,
                Players = new() { creator }
            };

            newMatch.SetRedPlayer(creator);
            newMatch.CurrentPlayer = creator;

            Console.WriteLine(newMatch.RedPlayer.UserName);

            Lobbies.Add(lobbyId, newMatch);

            return newMatch;
        }

        public void DeleteMatch(C4Match match)
        {
            var matchEntry = Lobbies.Values.FirstOrDefault(m => m.Id == match.Id);

            matchEntry.Players.Clear();

            matchEntry.Players.ForEach(p =>
            {
                p.Piece = C4Piece.Empty;
            });


            bool result = Lobbies.Remove(matchEntry.Id);
        }

        public void AddPlayerToLobby(C4Match match, C4Player player, C4Piece piece)
        {
            if (piece == C4Piece.Red)
            {
                match.SetRedPlayer(player);
            }
            else
            {
                match.SetYellowPlayer(player);
            }
        }

        public C4Match? GetUserMatch(string connectionId)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Players.FirstOrDefault(p => p.ConnectionId == connectionId) != null);
        }

        public bool PlacePiece(C4Match match, C4Player player, int column)
        {
            if (!match.IsPlayerTurn(player) || !match.Board.CanPlacePiece(new C4Move { Column = column }))
            {
                return false;
            }

            var move = new C4Move { Piece = player.Piece, Column = column };
            match.Board.PlacePiece(move);
            return true;
        }

        public C4Piece? CheckWinner(C4Match match)
        {
            var board = match.Board.Layout;
            int rows = match.Board.Rows;
            int cols = match.Board.Columns;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 3; col++)
                {
                    if (board[row][col] != C4Piece.Empty &&
                        board[row][col] == board[row][col + 1] &&
                        board[row][col] == board[row][col + 2] &&
                        board[row][col] == board[row][col + 3])
                    {
                        return board[row][col];
 
                    }
                }
            }

            // Vertical check
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 3; row++)
                {
                    if (board[row][col] != C4Piece.Empty &&
                        board[row][col] == board[row + 1][col] &&
                        board[row][col] == board[row + 2][col] &&
                        board[row][col] == board[row + 3][col])
                    {
                        return board[row][col];
                    }
                }
            }

            // top-left to bottom-right check
            for (int row = 0; row < rows - 3; row++)
            {
                for (int col = 0; col < cols - 3; col++)
                {
                    if (board[row][col] != C4Piece.Empty &&
                        board[row][col] == board[row + 1][col + 1] &&
                        board[row][col] == board[row + 2][col + 2] &&
                        board[row][col] == board[row + 3][col + 3])
                    {
                        return board[row][col];
                    }
                }
            }

            // top-right to bottom-left check
            for (int row = 0; row < rows - 3; row++)
            {
                for (int col = 3; col < cols; col++)
                {
                    if (board[row][col] != C4Piece.Empty &&
                        board[row][col] == board[row + 1][col - 1] &&
                        board[row][col] == board[row + 2][col - 2] &&
                        board[row][col] == board[row + 3][col - 3])
                    {
                        return board[row][col];
                    }
                }
            }

            return null;
        }

        public void ResetPlayer(C4Player player)
        {
            player.Piece = C4Piece.Empty;
        }


        public bool LobbyWithNameExists(string lobbyName)
        {
            return Lobbies.Any(l => l.Value.Name.Equals(lobbyName));
        }

        public C4Match? GetLobbyByName(string lobbyName)
        {
            return Lobbies.Values.FirstOrDefault(l => l.Name == lobbyName);
        }
    }
}
