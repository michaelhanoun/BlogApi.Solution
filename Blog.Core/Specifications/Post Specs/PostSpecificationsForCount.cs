using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Core.Specifications.Post_Specs
{
    public class PostSpecificationsForCount:BaseSpecifications<Post>
    {
        public PostSpecificationsForCount(PostSpecParam specParam) : base(P => ((!specParam.TagId.HasValue) || P.postTags.Any(PC => PC.Tag.Id == specParam.TagId.Value)) && (string.IsNullOrEmpty(specParam.CategorySlug) || P.PostCategories.Any(PC => PC.Category.Slug == specParam.CategorySlug.ToLower())) && (string.IsNullOrEmpty(specParam.Search) || P.Title.ToLower() == specParam.Search) && P.Status == Status.Published && (string.IsNullOrEmpty(specParam.AuthorId) || P.ApplicationUserId == specParam.AuthorId))
        {
            
        }
    }
}
