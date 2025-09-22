using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Specifications;

namespace Blog.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications);
        Task<T?> GetWithSpecAsync(ISpecifications<T> specifications);
        Task<T?> GetWithTrackingAndWithSpecAsync(ISpecifications<T> specifications);
        Task<int> GetCountAsync(ISpecifications<T> specifications);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
