using BZGames.Application.DTOs.Games.C4;

namespace BZGames.Application.DTOs.Games.TTT
{
    public class TTTMatch
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TTTPlayer> Players { get; set; } = new();
        public TTTBoard Board { get; set; } = new();
        public TTTPlayer XPlayer { get; set; } = null!;
        public TTTPlayer OPlayer { get; set; } = null!;
        public TTTPlayer CurrentPlayer { get; set; } = null!;

        public void SetXPlayer(TTTPlayer player)
        {
            XPlayer = player;
            player.Piece = TTTPiece.X;
            CurrentPlayer = player;
            Players.Add(player);
        }

        public void SetOPlayer(TTTPlayer player)
        {
            OPlayer = player;
            player.Piece = TTTPiece.O;
            Players.Add(player);
        }

        public bool IsPlayerTurn(TTTPlayer player) => CurrentPlayer?.ConnectionId == player.ConnectionId;

        public void SwitchTurn()
        {
            CurrentPlayer = CurrentPlayer.ConnectionId == XPlayer.ConnectionId ? OPlayer : XPlayer;
        }
    }
}
