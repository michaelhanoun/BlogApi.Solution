using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Blog.Infrastructure.Generic_Repository
{
    public static class SpecificationsEvaluator<T> where T:BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery ,ISpecifications<T> specifications)
        {
            var query = inputQuery;
            if (specifications.Criteria is not null)
             query= query.Where(specifications.Criteria);
            if (specifications.OrderByAsc is not null)
                query = query.OrderBy(specifications.OrderByAsc);
            if (specifications.OrderByDsc is not null)
                query = query.OrderBy(specifications.OrderByDsc);
            if(specifications.IsPaginationEnabled)
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            query = specifications.Includes.Aggregate(query,(currentQuery, includeExpression) =>includeExpression(currentQuery) );
            return query;
        }
    }
}
