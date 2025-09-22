namespace Blog.Api.Dtos
{
    public class PostDto 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
