using BZGames.Application.DTOs.Games.Common;
namespace BZGames.Application.DTOs.Games.BTS
{
    public class BTSPlayer : Player
    {
        public BTSPlayer(string userName, string connectionId) : base(userName, connectionId) 
        {
            Board = new();
        }

        public BTSBoard Board { get; set; }
    }
}
