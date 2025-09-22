using Blog.Api.Helper;

namespace Blog.Api.Dtos
{
    public class PostToCreateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public HashSet<int> Categories { get; set; } = new();
        public HashSet<int> Tags { get; set; } = new();
    }
}
