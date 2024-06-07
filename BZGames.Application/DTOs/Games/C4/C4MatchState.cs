namespace BZGames.Application.DTOs.Games.C4
{
    public class C4MatchState
    {
        public string Name { get; set; } = string.Empty;
        public List<C4Player> Players { get; private set; } = new List<C4Player>();
        public C4Board Board { get; private set; } = null!;
        public C4Player RedPlayer { get; private set; } = null!;
        public C4Player YellowPlayer { get; private set; } = null!;
        public C4Player CurrentPlayer { get; private set; } = null!;
        public C4Player Winner { get; set; } = null!;

        public C4MatchState(C4Player redPlayer, C4Player yellowPlayer, C4Board board, C4Player currentPlayer)
        {
            RedPlayer = redPlayer;
            YellowPlayer = yellowPlayer;
            Board = board;
            Players = new List<C4Player>() { RedPlayer, YellowPlayer };
            CurrentPlayer = currentPlayer;

        }
    }
}
