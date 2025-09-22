using System.Security.Claims;
using AutoMapper;
using Azure;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Specifications.Category;
using Blog.Core.Specifications.Post_Specs;
using Blog.Core.Specifications.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
namespace Blog.Api.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Cached(600)]

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAllCategories()
        {
            return Ok(_mapper.Map<IReadOnlyList<CategoryDto>>(await _unitOfWork.Repository<Category>().GetAllWithSpecAsync(new CategorySpecification())));
        }
        [Cached(600)]

        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(CategoryDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetWithSpecAsync(new CategorySpecification(id));
            if (category is null) return NotFound(new ApiResponse(404));
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
        [Cached(600)]

        [HttpGet("slug/{slug}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategory(string slug)
        {
            var category = await _unitOfWork.Repository<Category>().GetWithSpecAsync(new CategorySpecification(slug.ToLower()));
            if (category is null) return NotFound(new ApiResponse(404));
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryToCreateAndUpdateDto model)
        {
            Category category = new() { Name = model.Name, Slug = string.Empty };
            await _unitOfWork.Repository<Category>().Add(category);
            try
            { 
                int count = await _unitOfWork.CompleteAsync();
                if (count < 1)
                    return BadRequest(new ApiResponse(500, "Failed To Create Category"));
                category.Slug = $"{category.Id}-{SlugGenerator.GenerateSlug(category.Name)}";
                count = await _unitOfWork.CompleteAsync();
                if (count < 1)
                    return BadRequest(new ApiResponse(500, "Failed To Create Category"));
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            }
            catch {
                return Conflict(new ApiResponse(409, $"Category name {model.Name} already exists."));
            }
          

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<PostDto>> UpdateCategory(int id, CategoryToCreateAndUpdateDto model)
        {
            var category = await _unitOfWork.Repository<Category>().GetWithTrackingAndWithSpecAsync(new CategorySpecification(id));
            if (category is null)
                return NotFound(new ApiResponse(404, "Category not found."));
            category.Name = model.Name;
            category.Slug = $"{category.Id}-{SlugGenerator.GenerateSlug(category.Name)}";
            _unitOfWork.Repository<Category>().Update(category);
            try 
            {
                int count = await _unitOfWork.CompleteAsync();
                if (count < 1)
                    return BadRequest(new ApiResponse(500, "Failed To Update Category"));
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            } catch {
                return Conflict(new ApiResponse(409, $"Category name {model.Name} already exists."));

            }

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetWithTrackingAndWithSpecAsync(new CategorySpecification(id));
            if (category is null)
                return NotFound(new ApiResponse(404, "category not found."));
            _unitOfWork.Repository<Category>().Delete(category);
            int count = await _unitOfWork.CompleteAsync();
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Delete Category"));
            return NoContent();
        }
        [Cached(600)]

        [HttpGet("id/{id}/posts")]
        [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PostDto>>> GetCategoryPosts(int id)
        {
            return Ok(_mapper.Map<IReadOnlyList<PostDto>>(await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(new CategoryPostSpecification(id))));
        }
        [Cached(600)]
        [HttpGet("slug/{slug}/posts")]
        [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PostDto>>> GetCategoryPosts(string slug)
        {
            return Ok(_mapper.Map<IReadOnlyList<PostDto>>(await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(new CategoryPostSpecification(slug))));
        }

    }
}
