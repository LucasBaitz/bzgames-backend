namespace BZGames.Application.DTOs.Games.C4
{
    public class C4Board
    {
        public int Rows { get; set; } = 6;
        public int Columns { get; set; } = 7;
        public List<List<C4Piece>> Layout { get; private set; }

        public C4Board()
        {
            Layout = new List<List<C4Piece>>();

            // Initialize the list of lists
            for (int row = 0; row < Rows; row++)
            {
                Layout.Add(new List<C4Piece>());

                // Initialize each row with empty pieces
                for (int col = 0; col < Columns; col++)
                {
                    Layout[row].Add(C4Piece.Empty);
                }
            }
        }

        public bool IsFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (Layout[0][col] == C4Piece.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanPlacePiece(C4Move move)
        {
            return Layout[0][move.Column] == C4Piece.Empty;
        }

        public void PlacePiece(C4Move move)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (Layout[row][move.Column] == C4Piece.Empty)
                {
                    Layout[row][move.Column] = move.Piece;
                    break;
                }
            }
        }
    }
}
