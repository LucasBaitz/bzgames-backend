namespace BZGames.Infrastructure.Models
{
    public record UserContext
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string ConnectionId { get; init; } = string.Empty;
    }
}
