using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Post_Specs
{
    public class PostSpecification : BaseSpecifications<Post>
    {
        public PostSpecification(PostSpecParam specParam) : base(P => ((!specParam.TagId.HasValue) || P.postTags.Any(PC => PC.Tag.Id == specParam.TagId.Value)) && (string.IsNullOrEmpty(specParam.CategorySlug) || P.PostCategories.Any(PC => PC.Category.Slug == specParam.CategorySlug.ToLower())) && (string.IsNullOrEmpty(specParam.Search) || P.Title.ToLower() == specParam.Search) && P.Status == Status.Published && (string.IsNullOrEmpty(specParam.AuthorId) || P.ApplicationUserId == specParam.AuthorId))
        {
            AddIncludes();
            Sort(specParam);
            ApplyPagination((specParam.PageIndex-1) * specParam.PageSize,specParam.PageSize);
        }
        public PostSpecification(int id ,string slug) :base(p=>(p.Id == id && p.Slug == slug&& p.Status == Status.Published)) 
        {
            AddIncludes();
        }
        public PostSpecification(int id):base(P=>P.Id == id&& P.Status == Status.Published) 
        {
            AddIncludes();
        }
        private void AddIncludes()
        {
            Includes.Add(P => P.Include(p=>p.ApplicationUser));
            Includes.Add(P => P.Include(P=> P.Comments));
            Includes.Add(P => P.Include(P => P.PostCategories).ThenInclude(PC=>PC.Category));
            Includes.Add(P => P.Include(P => P.postTags).ThenInclude(PC=>PC.Tag));
            Includes.Add(P => P.Include(P => P.Likes));
        }
        private void Sort(PostSpecParam postSpecParam)
        {
            switch (postSpecParam.Sort)
            {
                case "TitleAsc":
                     AddOrderByAsc(P => P.Title);
                     break;

                case "TitleDsc":
                    AddOrderByDsc(P => P.Title);
                    break;

                case "SlugAsc":
                    AddOrderByAsc(P => P.Slug);
                    break;

                case "SlugDsc":
                    AddOrderByDsc(P => P.Slug);
                    break;
                default: AddOrderByDsc(P => P.UpdatedAt); break;
            }
        }
    }
}
