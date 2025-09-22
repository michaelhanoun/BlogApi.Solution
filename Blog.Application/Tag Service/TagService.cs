using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Services.Contracts;
using Blog.Core.Specifications.Tags;

namespace Blog.Application.Tag_Service
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddTagAsync(Tag tag)
        {
            await _unitOfWork.Repository<Tag>().Add(tag);
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<int> DeleteTagAsync(Tag tag)
        {
            _unitOfWork.Repository<Tag>().Delete(tag);
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<Post>> GetPosts(int id)
        {
            return await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(new TagPostSpecifications(id));
        }

        public async Task<Tag?> GetTag(int id)
        {
           return await _unitOfWork.Repository<Tag>().GetWithSpecAsync(new TagSpecification(id));
        }

        public async Task<IReadOnlyList<Tag>> GetTags()
        {
            return await _unitOfWork.Repository<Tag>().GetAllWithSpecAsync(new TagSpecification());
        }

        public async Task<Tag?> GetTagWithTracking(int id)
        {
           return await _unitOfWork.Repository<Tag>().GetWithTrackingAndWithSpecAsync(new TagSpecification(id));
        }
    }
}
