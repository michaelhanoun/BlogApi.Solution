using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Dtos
{
    public class ResetEmailPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{""':;'?/>\.<,])(?!.*\s).*$",
           ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non-alphanumeric character, and be 6–10 characters with no spaces.")]
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
