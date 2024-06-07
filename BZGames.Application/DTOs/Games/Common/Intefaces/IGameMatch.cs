using System;
namespace BZGames.Application.DTOs.Games.Common.Intefaces
{
    public interface IGameMatch<TPlayer> where TPlayer : Player
    {
        Guid Id { get; set; }
        string Name { get; set; }
        ICollection<TPlayer> Players { get; set; }
    }
}
