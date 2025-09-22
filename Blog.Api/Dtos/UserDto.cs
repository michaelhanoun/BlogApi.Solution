using System.ComponentModel.DataAnnotations;
using Blog.Core.Services.Contracts.Authentication_Service;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Blog.Api.Dtos
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public AuthDto AuthResponse { get; set; }
    }
}
