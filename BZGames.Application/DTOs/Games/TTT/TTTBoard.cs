using BZGames.Application.DTOs.Games.C4;

namespace BZGames.Application.DTOs.Games.TTT
{
    public class TTTBoard
    {
        public int Rows { get; set; } = 3;
        public int Columns { get; set; } = 3;
        public List<List<TTTPiece>> Layout { get; set; }

        public TTTBoard()
        {
            Layout = new List<List<TTTPiece>>();
            for (int row = 0; row < Rows; row++)
            {
                Layout.Add(new List<TTTPiece>());
                for (int col = 0; col < Columns; col++)
                {
                    Layout[row].Add(TTTPiece.Empty);
                }
            }
        }

        public bool IsFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (Layout[0][col] == TTTPiece.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanPlacePiece(TTTMove move)
        {
            return Layout[move.Row][move.Column] == TTTPiece.Empty;
        }

        public void PlacePiece(TTTMove move)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (CanPlacePiece(move))
                {
                    Layout[move.Row][move.Column] = move.Piece;
                    break;
                }
            }
        }
    }
}
