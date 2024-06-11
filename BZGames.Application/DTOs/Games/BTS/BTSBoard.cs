namespace BZGames.Application.DTOs.Games.BTS
{
    public class BTSBoard
    {
        private const int Rows = 10;
        private const int Columns = 10;
        public List<List<BTSPiece>> Layout { get; set; } = null!;

        public BTSBoard()
        {
            Layout = new List<List<BTSPiece>>();

            for (int row = 0; row < Rows; row++)
            {
                Layout.Add(new List<BTSPiece>());
                for (int col = 0; col < Columns; col++)
                {
                    Layout[row].Add(BTSPiece.Empty);
                }
            }
        }


        public void PlaceShips()
        {
            Random random = new Random();

            List<int> shipLengths = new List<int> { 2, 3, 3, 5, 5 };

            foreach (int length in shipLengths)
            {
                // Randomly select starting position
                int startRow = random.Next(0, Rows);
                int startCol = random.Next(0, Columns);

                bool isHorizontal = random.Next(0, 2) == 0;

                if (CanPlaceShip(startRow, startCol, length, isHorizontal))
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (isHorizontal)
                        {
                            Layout[startRow][startCol + i] = BTSPiece.Ship;
                        }
                        else
                        {
                            Layout[startRow + i][startCol] = BTSPiece.Ship;
                        }
                    }
                }
            }
        }

        public List<List<BTSPiece>> GetMaskedBoard()
        {
            List<List<BTSPiece>> maskedBoard = new List<List<BTSPiece>>();

            for (int i = 0; i < Layout.Count; i++)
            {
                List<BTSPiece> row = new List<BTSPiece>();
                for (int j = 0; j < Layout[i].Count; j++)
                {
                    if (Layout[i][j] == BTSPiece.Ship)
                    {
                        row.Add(BTSPiece.Empty);
                    }
                    else
                    {
                        row.Add(Layout[i][j]);
                    }
                }
                maskedBoard.Add(row);
            }

            return maskedBoard;
        }


        private bool CanPlaceShip(int startRow, int startCol, int length, bool isHorizontal)
        {
            if (isHorizontal && startCol + length > Columns)
            {
                return false;
            }
            else if (!isHorizontal && startRow + length > Rows)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (isHorizontal && Layout[startRow][startCol + i] == BTSPiece.Ship)
                {
                    return false;
                }
                else if (!isHorizontal && Layout[startRow + i][startCol] == BTSPiece.Ship)
                {
                    return false;
                }
            }

            return true;
        }

        public bool PlaceMove(int row, int col)
        {

            var boardTarget = Layout[row][col];

            if (boardTarget == BTSPiece.Hit || boardTarget == BTSPiece.Miss)
            {
                return true;
            }

            if (boardTarget == BTSPiece.Empty)
            {
                Layout[row][col] = BTSPiece.Miss;
                return false;
            }
            else if (boardTarget == BTSPiece.Ship)
            {
                Layout[row][col] = BTSPiece.Hit;
                return true;
            }

            return true;
        }


        public bool HasShips()
        {
            return Layout.Any(r => r.Any(p => p == BTSPiece.Ship));
        }
    }
}
