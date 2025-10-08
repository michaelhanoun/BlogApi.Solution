using System.Net;
using System.Security.Claims;
using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Services.Contracts;
using Blog.Core.Services.Contracts.Authentication_Service;
using Blog.Core.Specifications.Refresh_Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
   
    public class AuthController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticationService _authentication;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEmailSenderService _emailSender;
        private readonly IMapper _mapper;

        public AuthController(UserManager<ApplicationUser>userManager,SignInManager<ApplicationUser> signInManager,IAuthenticationService authentication,IUnitOfWork unitOfWork,IConfiguration configuration,ILogger<AuthController> logger,IEmailSenderService emailSender,IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authentication = authentication;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _emailSender = emailSender;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(U=>U.RefreshTokens).SingleOrDefaultAsync(U => U.Email == loginDto.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401, "Invalid login"));
            var response = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
            if (!response.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid login"));
            _authentication.RemoveOldRefreshTokens(user);
            var authResponse = await _authentication.AddRefreshTokenToUser(user, _userManager);
            if (authResponse is null)
                return BadRequest(new ApiResponse(500, "Something went wrong"));
            Response.Cookies.Append("refreshToken", authResponse.RefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = authResponse.RefreshToken.Expires
            });
            return Ok(new UserDto() {UserId = user.Id, Email = user.Email, AuthResponse = new() { AccessToken=authResponse.AccessToken,RefreshToken = Uri.EscapeDataString( authResponse.RefreshToken.Token)}, UserName = user.UserName });

        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            ApplicationUser applicationUser = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Bio = registerDto.Bio,
            };
            var response =await _userManager.CreateAsync(applicationUser, registerDto.Password);
            if (!response.Succeeded)
                return BadRequest(new ApiValidationErrorResponse() { Errors = response.Errors.Select(E => E.Description) });
            var authResponse = await _authentication.AddRefreshTokenToUser(applicationUser, _userManager);
            if (authResponse is null)
                return BadRequest(new ApiResponse(500,"Something went wrong"));

            Response.Cookies.Append("refreshToken", authResponse.RefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = authResponse.RefreshToken.Expires
            });
            return Ok(new UserDto() {UserId = applicationUser.Id, Email = applicationUser.Email, AuthResponse =  new() { AccessToken = authResponse.AccessToken, RefreshToken = Uri.EscapeDataString(authResponse.RefreshToken.Token) }, UserName = applicationUser.UserName });
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiResponse(400, "Refresh token is required"));
            refreshToken = Uri.UnescapeDataString(refreshToken);
            var token = await _unitOfWork.Repository<RefreshToken>().GetWithTrackingAndWithSpecAsync(new RefreshTokenSpecification(refreshToken));
            if (token is null || token.ApplicationUser is null)
               return Unauthorized(new ApiResponse(401, "Invalid Token"));
            if (token.Expires <= DateTime.UtcNow)
                return Unauthorized(new ApiResponse(401, "Expired Token"));

            Response.Cookies.Append("refreshToken", token.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.Expires
            });
           var authResponse =  await _authentication.RefreshAsync(token, token.ApplicationUser, _userManager);
            return Ok(new {authResponse.AccessToken, RefreshToken = Uri.EscapeDataString(authResponse.RefreshToken.Token)});
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string? refreshToken)
        {
            refreshToken ??= Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiResponse(400, "Refresh token is required"));
            var repo =  _unitOfWork.Repository<RefreshToken>();
            var token = await repo.GetWithSpecAsync(new RefreshTokenSpecification(refreshToken));
            if (token is null || token.ApplicationUser is null)
                return Unauthorized(new ApiResponse(401, "Invalid Token"));
            if (token.Expires <= DateTime.UtcNow)
                return Unauthorized(new ApiResponse(401, "Expired Token"));
            token.Revoked = DateTime.UtcNow;
            repo.Update(token);
            int count = await _unitOfWork.CompleteAsync();
            if (count < 1)
                return BadRequest(new ApiResponse(500,"Failed To Update Refresh Token"));
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully" });
        }
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users.Include(U=>U.RefreshTokens).SingleOrDefaultAsync(U=>U.Email == email);
            if (user is null) return NotFound(new ApiResponse(404));
            var refresh = user.RefreshTokens.Where(P => P.IsActive).ToList();
            if (!refresh.Any())
                return Ok(new { message = "No active sessions found" });
            foreach (var item in refresh)
            {
                item.Revoked = DateTime.UtcNow;
            }
            await _userManager.UpdateAsync(user);
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out successfully", revoked = refresh.Count });
        }
        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword(EmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordUrl = _configuration["ApiBaseUrl"] + Url.Action("ResetPassword", "Auth", new { email = user.Email, token });
                await _emailSender.SendEmailAsync(model.Email, "Reset Your Password", resetPasswordUrl);

            }
            return Ok(new { message = "the reset email will send to you if you registerd" });
        }
        [HttpPut("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetEmailPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await Task.Delay(500);
                return Ok(new { message = "If the provided data is valid, your password has been reset." });
            }

            var result = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(model.Token), model.Password);
            if (!result.Succeeded)
                _logger.LogWarning($"Password reset failed for {model.Email}. Error: {string.Join(",", result.Errors.Select(E => E.Description))}");
            return Ok(new { message = "If the provided data is valid, your password has been reset." });
        }
        [HttpGet("external-login")]
        public ActionResult ExternalLogin(string provider)
        {
            var redirecdUrl = _configuration["ApiBaseUrl"] + Url.Action("ExternalLoginCallback", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirecdUrl);
            return Challenge(properties, provider);
        }
        [HttpGet("external-login-callback")]
        public async Task<ActionResult<UserDto>> ExternalLoginCallback(string? returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Unauthorized(new ApiResponse(401, "Invalid Login"));
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            ApplicationUser user;
            if (result.Succeeded)
            {
                user = await _userManager.Users
             .Include(u => u.RefreshTokens)
             .SingleOrDefaultAsync(u => u.Email == info.Principal.FindFirstValue(ClaimTypes.Email));

                if (user == null)
                    return Unauthorized(new ApiResponse(401, "User not found"));

                _authentication.RemoveOldRefreshTokens(user);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var displayName = info.Principal.FindFirstValue(ClaimTypes.Name);
                var phone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);
                user = new ApplicationUser()
                {
                    Email = email,
                    UserName = displayName.Replace(" ",""),
                    Bio = string.Empty,
                    PhoneNumber = phone ?? "01xxxxxxxxx",
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                { return BadRequest(new ApiValidationErrorResponse() { Errors = createResult.Errors.Select(E => E.Description) }); }
                await _userManager.AddLoginAsync(user, info);
            }
            var authResponse = await _authentication.AddRefreshTokenToUser(user, _userManager);
            if (authResponse is null)
                return BadRequest(new ApiResponse(500, "Something went wrong"));
            Response.Cookies.Append("refreshToken", authResponse.RefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = authResponse.RefreshToken.Expires
            });
            return Ok(new UserDto() {UserId= user.Id, UserName = user.UserName, Email = user.Email,AuthResponse = new() { AccessToken = authResponse.AccessToken, RefreshToken = Uri.EscapeDataString(authResponse.RefreshToken.Token) } });
        }
        [Authorize]
        [Cached(600)]
        [ProducesResponseType(typeof(UserToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("Me")]
        public async Task<ActionResult<UserToReturnDto>> GetMe()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<UserToReturnDto>(user));
        }

    }
}
