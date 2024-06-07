namespace BZGames.Application.DTOs.Games.GRC
{
    public class GRCMatch
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<GRCPlayer> Players { get; set; } = null!;
        public GRCPlayer CurrentPlayer { get; set; } = null!;
        public List<GRCDrawngLine> CurrentDrawing { get; set; } = null!;
        public int CurrentMaxPoints { get; set; }
        public string CurrentWord { get; set; } = string.Empty;
        public List<string> Words { get; set; } = null!;    

    
        public void ResetMaxPoints()
        {
            CurrentMaxPoints = 10;
        }

        public void DescreseMaxPoints()
        {
            CurrentMaxPoints -= 2;
        }
    }
}
