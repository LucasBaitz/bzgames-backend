using BZGames.Application.DTOs.Games.C4;

namespace BZGames.Application.DTOs.Games.TTT
{
    public class TTTMatchState
    {
        public string Name { get; set; } = string.Empty;
        public List<TTTPlayer> Players { get; set; } = new();
        public TTTBoard Board { get; set; } = new();
        public TTTPlayer XPlayer { get; set; } = null!;
        public TTTPlayer OPlayer { get; set; } = null!;
        public TTTPlayer CurrentPlayer { get; set; } = null!;

        public TTTPlayer Winner { get; set; } = null!;

        public TTTMatchState(TTTPlayer xPlayer, TTTPlayer oPlayer, TTTBoard board, TTTPlayer currentPlayer)
        {
            XPlayer = xPlayer;
            OPlayer = oPlayer;
            Board = board;
            Players = new List<TTTPlayer>() { XPlayer, OPlayer };
            CurrentPlayer = currentPlayer;

        }
    }
}
