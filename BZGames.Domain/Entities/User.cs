using Microsoft.AspNetCore.Identity;

namespace BZGames.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Image { get; set; } = string.Empty;
        public double Rating { get; set; }
        public User(string userName, string email) : base(userName) 
        {
            Email = email;
        }

        public User(string userName, string email, string imagePath) : base(userName)
        {
            Email = email;
            Image = imagePath;
        }
    }
}
