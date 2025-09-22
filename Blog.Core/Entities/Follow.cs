using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Entities
{
    public class Follow
    {
        public string FollowerId { get; set; }
        public string FollowedId { get; set; }
        public ApplicationUser Follower { get; set; }
        public ApplicationUser Followed { get; set; }
    }
}
