using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Post_Specs
{
    public class PostDraftSpecification :BaseSpecifications<Post>
    {
        public PostDraftSpecification(int id):base (P=>P.Id == id)
        {
            AddIncludes();
            
        }
        private void AddIncludes()
        {
            Includes.Add(P => P.Include(p => p.ApplicationUser));
            Includes.Add(P => P.Include(P => P.Comments));
            Includes.Add(P => P.Include(P => P.PostCategories).ThenInclude(PC => PC.Category));
            Includes.Add(P => P.Include(P => P.postTags).ThenInclude(PC => PC.Tag));
            Includes.Add(P => P.Include(P => P.Likes));
        }
    }
}
