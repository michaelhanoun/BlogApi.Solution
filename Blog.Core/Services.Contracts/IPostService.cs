using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Specifications.Post_Specs;
using Microsoft.AspNetCore.Identity;

namespace Blog.Core.Services.Contracts
{
    public interface IPostService
    {
        Task<IReadOnlyList<Post>> GetPosts(PostSpecParam postSpecParam);
        Task<Post?> GetPost(int id , string slug);
        Task<Post?> GetAllTypeOfPostWithTracking(int id );
        Task<Post?> GetPost(int id);
        Task<Like?> GetLike(int id);
        Task<IReadOnlyList<Category>?> GetCategories(HashSet<int> ids);
        Task<IReadOnlyList<Tag>?> GetTags(HashSet<int> ids);
        Task<int> GetPostsCount(PostSpecParam postSpecParam);
        Task<int> GetLikesCount(string userId,int postId);
        Task<IReadOnlyList<Comment>> GetCommentsForPostById(int id);
        Task<IReadOnlyList<Like>> GetLikesForPostById(int id);
        Task<int> AddAsync<T>(T Entity) where T : BaseEntity;
        Task<int> UpdateAsync<T>(T Entity) where T:BaseEntity;
        Task<int> DeleteAsync<T>(T Entity) where T : BaseEntity;
    }
}
