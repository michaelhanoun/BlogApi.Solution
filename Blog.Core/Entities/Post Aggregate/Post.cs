using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Entities.Post_Aggregate
{
    public class Post:BaseEntity
    {
        public string ApplicationUserId { get; set; } 
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public Status Status { get; set; } = Status.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public ICollection<PostTag> postTags { get; set; } = new HashSet<PostTag>();

    }
}
