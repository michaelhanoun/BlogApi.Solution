using System.Security.Claims;
using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Api.Hubs;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Services.Contracts;
using Blog.Core.Specifications.Likes;
using Blog.Core.Specifications.Post_Specs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{

    public class PostsController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PostsController(IPostService postService, IMapper mapper,UserManager<ApplicationUser>userManager, IHubContext<NotificationHub> hubContext)
        {
            _postService = postService;
            _mapper = mapper;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<PostDto>>> GetPosts([FromQuery] PostSpecParam postSpecParam)
        {
            var posts = await _postService.GetPosts(postSpecParam);
            var postsDto = _mapper.Map<IReadOnlyList<PostDto>>(posts);
            var count = await _postService.GetPostsCount(postSpecParam);
            return Ok(new Pagination<PostDto>(postSpecParam.PageIndex, postSpecParam.PageSize, count, postsDto));
        }
        [Cached(600)]
        [HttpGet("{id}/{slug}")]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PostDto),StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<PostDto>>> GetPost(int id , string slug)
        {
            var post = await _postService.GetPost(id,slug);
            if (post is null)
                return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<PostDto>(post));
        }
        [Cached(600)]

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetComments(int id)
        {
            var post = await _postService.GetPost(id);
            if (post is null)
                return NotFound(new ApiResponse(404));
            return Ok (_mapper.Map<IReadOnlyList<CommentDto>>(await _postService.GetCommentsForPostById(id)));
        }
        [Authorize]
        [HttpPost("{id}/comments")]
        public async Task<ActionResult<CommentDto>> CreateComment(int id,CommentToCreateDto model)
        {
            var post = await _postService.GetPost(id);
            if (post is null)
                return NotFound(new ApiResponse(404));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var comment = new Comment() {Content = model.Content,PostId = id,ApplicationUserId=userId,CreatedAt=DateTime.UtcNow};
           int count = await _postService.AddAsync(comment);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Create Comment"));
            return Ok(_mapper.Map<CommentDto>(comment));

        }
        [Cached(600)]

        [HttpGet("{id}/likes")]
        public async Task<ActionResult<IReadOnlyList<LikeDto>>> GetLikes(int id)
        {
            var post = await _postService.GetPost(id);
            if (post is null)
                return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<IReadOnlyList<LikeDto>>(await _postService.GetLikesForPostById(id)));
        }
        [Authorize]
        [HttpPost("{id}/likes")]
        public async Task<ActionResult<CommentDto>> CreateLikes(int id)
        {
            var post = await _postService.GetPost(id);
            if (post is null)
                return NotFound(new ApiResponse(404));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var count = await _postService.GetLikesCount(user.Id,post.Id);
            if (count > 0)
                return Conflict(new ApiResponse(409, "You already liked this post."));
            var like = new Like() { PostId = id, ApplicationUserId = userId, CreatedAt = DateTime.UtcNow, };
            int changeCount = await _postService.AddAsync(like);
            if (changeCount < 1)
                return BadRequest(new ApiResponse(500, "Failed To Create Like"));
            return Ok(_mapper.Map<LikeDto>(like));

        }
        [Authorize]
        [HttpDelete("{id}/likes")]
        public async Task<IActionResult> DeleteLikes(int id)
        {
            var like = await _postService.GetLike(id);
            if(like is null)
                return NotFound(new ApiResponse(404));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            if (userId != like.ApplicationUserId)
                return Unauthorized(new ApiResponse(401));
            int count = await _postService.DeleteAsync(like);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Delete count"));
            return NoContent();
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost(PostToCreateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var categories = await _postService.GetCategories(model.Categories);
            if (categories is null)
                return BadRequest(new ApiResponse(400, "Some categories not found"));
            var tags = await _postService.GetTags(model.Tags);
            if (tags is null)
                return BadRequest(new ApiResponse(400, "Some Tags not found"));
            var post  = new Post() {ApplicationUserId = userId,Title = model.Title,Slug = SlugGenerator.GenerateSlug(model.Title),Content =model.Content,CreatedAt=DateTime.UtcNow,Status=Status.Draft,PostCategories = categories.Select(C=>new PostCategory() {CategoryId = C.Id}).ToHashSet(),postTags = tags.Select(T=>new PostTag() {TagId = T.Id}).ToHashSet()};
            int count = await _postService.AddAsync(post);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Create Post"));
            return Ok(_mapper.Map<PostDto>(post));
        }
        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<PostDto>> UpdatePost(int id,PostToUpdateDto model)
        {

            var post = await _postService.GetAllTypeOfPostWithTracking(id);
            if (post is null)
                return NotFound(new ApiResponse(404, "Post not found."));
            var categories = await _postService.GetCategories(model.Categories);
            if (categories is null)
                return BadRequest(new ApiResponse(400, "Some categories not found"));
            var tags = await _postService.GetTags(model.Tags);
            if (tags is null)
                return BadRequest(new ApiResponse(400, "Some Tags not found"));
            post.Title = model.Title; 
            post.Slug = SlugGenerator.GenerateSlug(model.Title);
            post.Content = model.Content;
            post.UpdatedAt = DateTime.UtcNow;
            post.Status = model.Status;
            post.PostCategories = categories.Select(C => new PostCategory() { CategoryId = C.Id ,PostId = post.Id }).ToHashSet();
            post.postTags = tags.Select(T => new PostTag() { TagId = T.Id ,PostId = post.Id}).ToHashSet();
            int count = await _postService.UpdateAsync(post);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Update Post"));
            return Ok(_mapper.Map<PostDto>(post));
        }
        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postService.GetAllTypeOfPostWithTracking(id);
            if (post is null)
                return NotFound(new ApiResponse(404, "Post not found."));
           int count = await _postService.DeleteAsync(post);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Delete Post"));
            return NoContent();
        }
        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpPatch("{id}/publish")]
        public async Task<ActionResult<PostDto>>UpdatePostStatus(int id , UpdatePostStatusDto model)
        {
            var post = await _postService.GetAllTypeOfPostWithTracking(id);
            if (post is null)
                return NotFound(new ApiResponse(404, "Post not found."));
            post.UpdatedAt = DateTime.UtcNow;
            post.Status = model.Status;
            int count = await _postService.UpdateAsync(post);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Update Post"));
            if (model.Status == Status.Published)
            {
                var user = await _userManager.Users.Include(U => U.Followers).FirstOrDefaultAsync(U => U.Id == post.ApplicationUserId);
                if (user is null)
                    return BadRequest(new ApiResponse(404, "the post doesn't have user"));
                foreach (var item in user.Followers)
                {
                    await _hubContext.Clients.User(item.FollowerId).SendAsync("ReceiveNotification", $"there is new post uploaded by {user.UserName}");
                }
            }
            return Ok(_mapper.Map<PostDto>(post));
        }
    }
}

