using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Likes
{
    public class LikeSpecifications:BaseSpecifications<Like>
    {
        public LikeSpecifications(int id):base(L=>L.PostId == id)
        {
            Includes.Add(Q=>Q.Include(C=>C.ApplicationUser));
        }
        public LikeSpecifications(string userId,int postId):base(L=>L.PostId==postId&&L.ApplicationUserId==userId)
        {
            
        }
    }
}
