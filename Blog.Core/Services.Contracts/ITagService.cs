using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Core.Services.Contracts
{
    public interface ITagService
    {
        Task<IReadOnlyList<Tag>> GetTags();
        Task<Tag?> GetTag(int id);
        Task<IReadOnlyList<Post>> GetPosts(int id);
        Task<Tag?> GetTagWithTracking(int id);
        Task<int> AddTagAsync(Tag tag);
        Task<int> DeleteTagAsync(Tag tag);
    }
}
