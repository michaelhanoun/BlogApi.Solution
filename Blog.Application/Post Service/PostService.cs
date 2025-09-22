using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Services.Contracts;
using Blog.Core.Specifications.Category;
using Blog.Core.Specifications.Comments;
using Blog.Core.Specifications.Likes;
using Blog.Core.Specifications.Post_Specs;
using Blog.Core.Specifications.Tags;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Blog.Application.Product_Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Category>?> GetCategories(HashSet<int> ids)
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllWithSpecAsync(new CategorySpecification(ids));
            if (categories.Count != ids.Count)
                return null;
            return categories;
        }
        public async Task<IReadOnlyList<Comment>> GetCommentsForPostById(int id)
        {
            return await _unitOfWork.Repository<Comment>().GetAllWithSpecAsync(new CommentSpecification(id));
        }
        public async Task<Like?> GetLike(int id)
        {
            return await _unitOfWork.Repository<Like>().GetWithSpecAsync(new LikeSpecificationToGetLike(id));
        }
        public async Task<IReadOnlyList<Tag>?> GetTags(HashSet<int> ids)
        {
            var tags = await _unitOfWork.Repository<Tag>().GetAllWithSpecAsync(new TagSpecification(ids));
            if (tags.Count != ids.Count)
                return null;
            return tags;
        }
        public async Task<IReadOnlyList<Like>> GetLikesForPostById(int id)
        {
            return await _unitOfWork.Repository<Like>().GetAllWithSpecAsync(new LikeSpecifications(id));
        }
        public async Task<Post?> GetPost(int id, string slug)
        {
            return await _unitOfWork.Repository<Post>().GetWithSpecAsync(new PostSpecification(id,slug));
        }
        public async Task<Post?> GetPost(int id)
        {
            return await _unitOfWork.Repository<Post>().GetWithSpecAsync(new PostSpecification(id));
        }
        public async Task<IReadOnlyList<Post>> GetPosts(PostSpecParam postSpecParam)
        {
            return await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(new PostSpecification(postSpecParam));
           
        }
        public async Task<int> GetPostsCount(PostSpecParam postSpecParam)
        {
            return await _unitOfWork.Repository<Post>().GetCountAsync(new PostSpecificationsForCount(postSpecParam));
        }
        public async Task<Post?> GetAllTypeOfPostWithTracking(int id)
        {
            return await _unitOfWork.Repository<Post>().GetWithTrackingAndWithSpecAsync(new PostDraftSpecification(id));
        }
        public async Task<int> GetLikesCount(string userId, int postId)
        {
           return await _unitOfWork.Repository<Like>().GetCountAsync(new LikeSpecifications(userId, postId));
        }
        public async Task<int> AddAsync<T>(T Entity) where T : BaseEntity
        {
            await _unitOfWork.Repository<T>().Add(Entity);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<int> UpdateAsync<T>(T Entity) where T : BaseEntity
        {
            _unitOfWork.Repository<T>().Update(Entity);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<int> DeleteAsync<T>(T Entity) where T : BaseEntity
        {
            _unitOfWork.Repository<T>().Delete(Entity);
            return await _unitOfWork.CompleteAsync();
        }
    }
}
