using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Core.Services.Contracts.Authentication_Service
{
    public interface IAuthenticationService
    {
        public void RemoveOldRefreshTokens(ApplicationUser user);
        public  Task<AuthResponse> AddRefreshTokenToUser(ApplicationUser user, UserManager<ApplicationUser> _userManager);
        Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser,UserManager<ApplicationUser> manager);
        RefreshToken CreateRefreshToken();
        public Task<AuthResponse> RefreshAsync(RefreshToken oldToken, ApplicationUser applicationUser, UserManager<ApplicationUser> userManager);
    }
}
