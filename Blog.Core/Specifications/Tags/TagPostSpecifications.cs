using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Tags
{
    public class TagPostSpecifications:BaseSpecifications<Post>
    {
        public TagPostSpecifications(int id):base(T=>T.postTags.Any(PT=>PT.Tag.Id==id) && T.Status == Status.Published)
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
