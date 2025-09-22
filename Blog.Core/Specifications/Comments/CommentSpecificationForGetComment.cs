using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;

namespace Blog.Core.Specifications.Comments
{
    public class CommentSpecificationForGetComment:BaseSpecifications<Comment>
    {
        public CommentSpecificationForGetComment(int id):base(C=>C.Id==id)
        {
            
        }
    }
}
