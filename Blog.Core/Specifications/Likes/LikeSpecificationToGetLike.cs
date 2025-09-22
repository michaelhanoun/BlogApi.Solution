using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;

namespace Blog.Core.Specifications.Likes
{
    public class LikeSpecificationToGetLike : BaseSpecifications<Like>
    {
        public LikeSpecificationToGetLike(int id):base(L=>L.Id==id)
        {
            
        }
    }
}
