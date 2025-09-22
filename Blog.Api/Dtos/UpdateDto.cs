using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Dtos
{
    public class UpdateDto
    {

        public string? UserName { get; set; }
        public string? Bio { get; set; }
    }
}
