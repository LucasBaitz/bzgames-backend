using BZGames.Application.Interfaces.Services;
using BZGames.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BZGames.Infrastructure.Security.Token
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly SymmetricSecurityKey _securityKey;

        public JwtTokenGenerator()
        {
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TH3@B357!53CUR17Y--K3Y$%3VERM4DEF0RSUR3"));
        }

        public string GenerateToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new("id", user.Id.ToString()),
                new("userName", user.UserName!),
                new("loginTimeStamp", DateTime.UtcNow.ToString())
            };

            var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "BZGames",
                expires: DateTime.Now.AddHours(24),
                claims: claims,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
