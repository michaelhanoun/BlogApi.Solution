using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Services.Contracts;
using Blog.Core.Specifications.Category;
using Blog.Core.Specifications.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    
    public class TagsController : BaseController
    {
        private readonly ITagService _tagService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagsController(ITagService tagService,IUnitOfWork unitOfWork , IMapper mapper)
        {
            _tagService = tagService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Cached(600)]

        [HttpGet]
        
        public async Task<ActionResult<IReadOnlyList<TagDto>>> GetTags()
        {
            return Ok(_mapper.Map<IReadOnlyList<TagDto>>(await _tagService.GetTags()));
        }
        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TagDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagDto>> GetTag(int id)
        {
            var tag = await _tagService.GetTag(id);
            if (tag is null) return NotFound(new ApiResponse(404));
            var tagDto = _mapper.Map<TagDto>(tag);
            return Ok(tagDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag(TagToCreateDto model)
        {
            Tag tag = new() { Name = model.Name};
            int count = await _tagService.AddTagAsync(tag);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Create Tag"));
            return Ok(_mapper.Map<TagDto>(tag));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _tagService.GetTagWithTracking(id);
            if (tag is null)
                return NotFound(new ApiResponse(404, "tag not found."));
            int count = await _tagService.DeleteTagAsync(tag);
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Delete Tag"));
            return NoContent();
        }
        [Cached(600)]
        [HttpGet("{id}/posts")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<PostDto>>> GetPostsWithTagId(int id)
        {
            return Ok(_mapper.Map<IReadOnlyList<PostDto>>(await _tagService.GetPosts(id)));
        }
    }
}
