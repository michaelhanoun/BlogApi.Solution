using AutoMapper;
using Blog.Api.Dtos;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;

namespace Blog.Api.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDto>()
                                      .ForMember(P => P.UserName, O => O.MapFrom(P => P.ApplicationUser.UserName))
                                      .ForMember(P => P.UserId, O => O.MapFrom(P => P.ApplicationUser.Id))
                                      .ForMember(P => P.LikesCount, O => O.MapFrom(P => P.Likes.Count))
                                      .ForMember(P => P.CommentsCount, O => O.MapFrom(O => O.Comments.Count))
                                      .ForMember(P => P.Categories, O => O.MapFrom(P=>P.PostCategories.Select(c=> c.Category == null ? c.CategoryId.ToString() : c.Category.Name).ToList()))
                                      .ForMember(P => P.Tags, O => O.MapFrom(P=>P.postTags.Select(c=>c.Tag == null?c.TagId.ToString(): c.Tag.Name).ToList()));
            CreateMap<Category, CategoryDto>().ForMember(C => C.PostCount, o => o.MapFrom(C => C.PostCategories.Count));
            CreateMap<Comment, CommentDto>().ForMember(C => C.UserName, o => o.MapFrom(C => C.ApplicationUser == null? "":C.ApplicationUser.UserName));
            CreateMap<Like, LikeDto>().ForMember(C => C.UserName, o => o.MapFrom(C => C.ApplicationUser == null ? "" : C.ApplicationUser.UserName));
            CreateMap<Tag, TagDto>().ForMember(C => C.PostCount, o => o.MapFrom(C => C.postTags.Count));
            CreateMap<ApplicationUser, UserToReturnDto>();
            CreateMap<Follow,FollowToReturnDto>().ForMember(F=>F.UserName,O=>O.MapFrom(F=>F.Followed.UserName));
        }
    }
}
