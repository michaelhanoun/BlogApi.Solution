using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Category
{
    public class CategorySpecification:BaseSpecifications<Blog.Core.Entities.Category>
    {
        public CategorySpecification()
        {
            Includes.Add(Q=>Q.Include(C=>C.PostCategories));
        }
        public CategorySpecification(int id):base(C=>C.Id == id)
        {
            Includes.Add(Q => Q.Include(C => C.PostCategories));
        }
        public CategorySpecification(string slug) : base(C => C.Slug == slug)
        {
            Includes.Add(Q => Q.Include(C => C.PostCategories));
        }
        public CategorySpecification(HashSet<int> Ids):base(C=>Ids.Contains(C.Id))
        {
            
        }
    }
}
