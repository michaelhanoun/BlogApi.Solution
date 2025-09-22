using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Blog.Core.Specifications
{
    public interface ISpecifications<T> where T:BaseEntity
    {
        public Expression<Func<T,bool>> Criteria { get; set; }
        public List<Func<IQueryable<T>,IIncludableQueryable<T,object>>> Includes { get; set; }

        public Expression<Func<T,object>> OrderByAsc { get; set; }
        public Expression<Func<T,object>> OrderByDsc { get; set; }
        public bool IsPaginationEnabled { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
