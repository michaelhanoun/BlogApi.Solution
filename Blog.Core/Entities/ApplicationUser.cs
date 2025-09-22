using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.AspNetCore.Identity;

namespace Blog.Core.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Post> Posts { get; set; } =new HashSet<Post>();
        public ICollection<Like> Likes { get; set; } =new HashSet<Like>();
        public ICollection<Comment> Comments { get; set; } =new HashSet<Comment>();
        public ICollection<Follow> Followers { get; set; } =new HashSet<Follow>();
        public ICollection<Follow> Following { get; set; } =new HashSet<Follow>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } =new HashSet<RefreshToken>();


    }
}
