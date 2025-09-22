using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Entities
{
    public class Tag :BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PostTag> postTags { get; set; } = new HashSet<PostTag>();
    }
}
