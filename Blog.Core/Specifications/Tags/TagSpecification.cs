using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Tags
{
    public class TagSpecification : BaseSpecifications<Tag>
    {
        public TagSpecification()
        {
            Includes.Add(Q=>Q.Include(T=>T.postTags));
        }
        public TagSpecification(int id):base(T=>T.Id==id)
        {
            Includes.Add(Q=>Q.Include(T=>T.postTags));
        }
        public TagSpecification(HashSet<int>Ids):base(C=>Ids.Contains(C.Id))
        {
            
        }
    }
}
