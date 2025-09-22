using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Comments
{
    public class CommentSpecification :BaseSpecifications<Comment>
    {
        public CommentSpecification(int id):base(C=>C.PostId == id)
        {
            Includes.Add(P => P.Include(C => C.ApplicationUser));
        }
    }
}
