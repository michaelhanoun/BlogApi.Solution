using Blog.Core.Entities;

namespace Blog.Api.Dtos
{
    public class AuthDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
