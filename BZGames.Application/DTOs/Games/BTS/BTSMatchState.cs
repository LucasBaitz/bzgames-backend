namespace BZGames.Application.DTOs.Games.BTS
{
    public class BTSMatchState
    {
        public BTSPlayer PlayerOne { get; private set; } = null!;
        public BTSPlayer PlayerTwo { get; private set; } = null!;
        public BTSPlayer CurrentPlayer { get; set; } = null!;
        public BTSBoard OpponentBoard { get; set; } = new();

        public BTSMatchState(BTSPlayer playerOne, BTSPlayer playerTwo, BTSPlayer currentPlayer, BTSBoard opponentBoard)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            CurrentPlayer = currentPlayer;
            OpponentBoard.Layout = opponentBoard.GetMaskedBoard();
        }
    }
}
