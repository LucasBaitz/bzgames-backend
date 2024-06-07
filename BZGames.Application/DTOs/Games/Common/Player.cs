namespace BZGames.Application.DTOs.Games.Common
{
    public class Player
    {
        public string UserName { get; private set; } = string.Empty;
        public string ConnectionId { get; private set; } = string.Empty;

        public Player(string userName, string connectionId)
        {
            UserName = userName;
            ConnectionId = connectionId;
        }
    }
}
