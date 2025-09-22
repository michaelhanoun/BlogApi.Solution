using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Repositories.Contract;
using Blog.Core.Specifications;
using Blog.Infrastructure._Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Blog.Infrastructure.Generic_Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly BlogContext _blogContext;

        public GenericRepository(BlogContext blogContext)
        {
            _blogContext = blogContext;
        }
        public async Task Add(T entity)
        {
          await  _blogContext.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _blogContext.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecifications(specifications).AsNoTracking().ToListAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecifications(specifications).CountAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecifications(specifications).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<T?> GetWithTrackingAndWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecifications(specifications).FirstOrDefaultAsync();
        }

        public void Update(T entity)
        {
            _blogContext.Update(entity);
        }
        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
        {
            return SpecificationsEvaluator<T>.GetQuery(_blogContext.Set<T>(), specifications);
        }
    }
}
