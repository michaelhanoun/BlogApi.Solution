using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Services.Contracts;
using Blog.Core.Services.Contracts.Authentication_Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Application.Auth_Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser, UserManager<ApplicationUser> manager)
        {
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name,applicationUser.UserName),
                                             new Claim(ClaimTypes.Email,applicationUser.Email),
                                             new Claim(ClaimTypes.NameIdentifier,applicationUser.Id),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
};
            var userRole = await manager.GetRolesAsync(applicationUser);
            foreach (var item in userRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            
            var jwtSecurityToken = new JwtSecurityToken(expires:DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "30")),issuer: _configuration["Jwt:Issuer"],audience: _configuration["Jwt:Audience"], claims:claims,signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]??string.Empty)),SecurityAlgorithms.HmacSha256Signature));
            
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public RefreshToken CreateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["Refresh:DurationInDays"] ?? "7")),
                Created = DateTime.UtcNow
            };
        }
        public void RemoveOldRefreshTokens(ApplicationUser user)
        {
            if (user.RefreshTokens == null || !user.RefreshTokens.Any())
                return;
            int maxTokens =int.Parse( _configuration["Refresh:MaxRefreshTokens"]??"5"); 

            var expired = user.RefreshTokens
                .Where(rt => !rt.IsActive)
                .ToList();

            foreach (var token in expired)
                user.RefreshTokens.Remove(token);

            if (user.RefreshTokens.Count > maxTokens)
            {
                var oldest = user.RefreshTokens.OrderBy(rt => rt.Created).First();
                user.RefreshTokens.Remove(oldest);
            }
        }
        public async Task<AuthResponse?> RefreshAsync(RefreshToken oldToken,ApplicationUser applicationUser,UserManager<ApplicationUser>userManager)
        {
            oldToken.Revoked = DateTime.UtcNow;
            var refreshToken = CreateRefreshToken();
            oldToken.ReplacedByToken = refreshToken.Token;
            applicationUser.RefreshTokens.Add(refreshToken);
            var result = await userManager.UpdateAsync(applicationUser);
            if(!result.Succeeded) return null;
            var newAccessToken = await CreateJwtTokenAsync(applicationUser,userManager);
            return new AuthResponse()
            {
                RefreshToken = refreshToken,
                AccessToken = newAccessToken
            };
        }

        public async Task<AuthResponse?> AddRefreshTokenToUser(ApplicationUser user,UserManager<ApplicationUser>_userManager)
        {
            var rToken = CreateRefreshToken();
            user.RefreshTokens.Add(rToken);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return null;

            var token = await CreateJwtTokenAsync(user, _userManager);
            return new () { AccessToken = token, RefreshToken = rToken };
        }
    }
}
