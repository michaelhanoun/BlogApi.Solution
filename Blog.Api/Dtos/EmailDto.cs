using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Dtos
{
    public class EmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
