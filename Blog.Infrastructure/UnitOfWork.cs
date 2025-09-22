using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Repositories.Contract;
using Blog.Infrastructure._Data;
using Blog.Infrastructure.Generic_Repository;

namespace Blog.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogContext _blogContext;
        private Hashtable _repositories;
        public UnitOfWork(BlogContext blogContext)
        {
            _blogContext = blogContext;
            _repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        {
           return await _blogContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _blogContext.DisposeAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var typeName = typeof(T).Name;
            if (!_repositories.ContainsKey(typeName))
                _repositories.Add(typeName,new GenericRepository<T>(_blogContext));
            return _repositories[typeName] as IGenericRepository<T>;
        }
    }
}
