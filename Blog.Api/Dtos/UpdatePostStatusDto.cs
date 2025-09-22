using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Api.Dtos
{
    public class UpdatePostStatusDto
    {
        public Status Status { get; set; }
    }
}
