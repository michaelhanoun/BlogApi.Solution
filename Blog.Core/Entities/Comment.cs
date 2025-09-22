using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Core.Entities
{
    public class Comment:BaseEntity
    {
        public int PostId { get; set; }
        public string? ApplicationUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Post Post { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
