namespace BZGames.Application.DTOs.Games.C4
{
    public class C4Match
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<C4Player> Players { get; set; } = new();
        public C4Board Board { get;  set; } = new();
        public C4Player RedPlayer { get; set; } = null!;
        public C4Player YellowPlayer { get; set; } = null!;
        public C4Player CurrentPlayer { get; set; } = null!;

        public void SetRedPlayer(C4Player player)
        {
            RedPlayer = player;
            player.Piece = C4Piece.Red;
            CurrentPlayer = player;
            Players.Add(player);
        }

        public void SetYellowPlayer(C4Player player)
        {
            YellowPlayer = player;
            player.Piece = C4Piece.Yellow; 
            Players.Add(player);
        }

        public bool IsPlayerTurn(C4Player player) => CurrentPlayer?.ConnectionId == player.ConnectionId;

        public void SwitchTurn()
        {
            CurrentPlayer = CurrentPlayer.ConnectionId == RedPlayer.ConnectionId ? YellowPlayer : RedPlayer;
        }

        public override string ToString()
        {
            return $"CurrentPlayer: {CurrentPlayer.UserName}, RedPlayer: {RedPlayer.UserName}, YellowPlayer{YellowPlayer.UserName}";
        }
    }
}
