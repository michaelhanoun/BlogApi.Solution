using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Api.Dtos
{
    public class PostToUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Status Status { get; set; } 
        public HashSet<int> Categories { get; set; }
        public HashSet<int> Tags { get; set; }
    }
}
