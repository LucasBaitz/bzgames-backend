namespace BZGames.Application.DTOs.Games.BTS
{
    public class BTSMatch
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<BTSPlayer> Players { get; set; } = new();
        public BTSPlayer PlayerOne { get; set; } = null!;
        public BTSPlayer PlayerTwo { get; set; } = null!;
        public BTSPlayer CurrentPlayer { get; set; } = null!;

        public void SwapTurn()
        {
            CurrentPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;
        }
    }
}
