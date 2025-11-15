using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.DTOs;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using WorldModel;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<WorldModelUsers> userManager, JwtHandler jwtHandler) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            WorldModelUsers? worldUser = await userManager.FindByNameAsync(loginRequest.Username);

            if (worldUser == null)
            {
                return Unauthorized("Invalid username");
            }

            bool loginStatus = await userManager.CheckPasswordAsync(worldUser, loginRequest.Password);
            if (!loginStatus)
            {
                return Unauthorized("Invalid password");
            }

            JwtSecurityToken token = await jwtHandler.GenerateTokenAsync(worldUser);

            string stringToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = stringToken
            });

        }
    }
}
