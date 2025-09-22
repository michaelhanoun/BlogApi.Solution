using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Category
{
    public class CategoryPostSpecification:BaseSpecifications<Post>
    {
        public CategoryPostSpecification(int id):base(C=>C.PostCategories.Any(PC=>PC.Category.Id == id) && C.Status == Status.Published)
        {
            AddIncludes();
        }
        public CategoryPostSpecification(string slug) : base(C=>C.PostCategories.Any(PC => PC.Category.Slug == slug)&&C.Status==Status.Published)
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
