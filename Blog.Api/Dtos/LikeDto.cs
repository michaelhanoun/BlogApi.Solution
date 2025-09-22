namespace Blog.Api.Dtos
{
    public class LikeDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
    }
}
