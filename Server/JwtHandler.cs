using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldModel;

namespace Server
{
    public class JwtHandler(UserManager<WorldModelUsers> userManager, IConfiguration configuration)
    {
        public async Task<JwtSecurityToken> GenerateTokenAsync(WorldModelUsers user)
        {
            return new JwtSecurityToken
            (
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: GetSigningCredentials(),
                claims: await GetClaimsAsync(user)
            );
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);
            SymmetricSecurityKey signingKey = new (key);
            
            return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(WorldModelUsers user)
        {
            List<Claim> claims = [new Claim(ClaimTypes.Name, user.UserName!)];
         
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
