namespace BZGames.API.Interfaces
{
    public interface ILobbyManager<TGameLobbie> where TGameLobbie : IGameLobby
    {
        Dictionary<Guid, TGameLobbie> ActiveLobbies { get; }
        Task CreateLobby();
        Task DeleteLobby();

    }
}
