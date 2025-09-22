using Blog.Core.Entities;

namespace Blog.Api.Dtos
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; } 
    }
}
