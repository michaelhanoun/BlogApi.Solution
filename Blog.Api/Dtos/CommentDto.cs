using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Api.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
    }
}
