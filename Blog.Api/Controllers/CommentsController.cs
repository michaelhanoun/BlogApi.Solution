using System.Security.Claims;
using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Specifications.Comments;
using Blog.Core.Specifications.Likes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    
    public class CommentsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CommentsController(IUnitOfWork unitOfWork , UserManager<ApplicationUser>userManager,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id,CommentToCreateDto commentDto) {
            var comment = await _unitOfWork.Repository<Comment>().GetWithTrackingAndWithSpecAsync(new CommentSpecificationForGetComment(id));
            if(comment == null)
                return NotFound(new ApiResponse(404));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound(new ApiResponse(404));
            if (userId != comment.ApplicationUserId)
                return Unauthorized(new ApiResponse(401));
            comment.Content = commentDto.Content;
             _unitOfWork.Repository<Comment>().Update(comment);
            int count = await _unitOfWork.CompleteAsync();
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Update Comment"));
            return Ok(_mapper.Map<CommentDto>(comment));
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _unitOfWork.Repository<Comment>().GetWithSpecAsync(new CommentSpecificationForGetComment(id));
            if (comment is null)
                return NotFound(new ApiResponse(404));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound(new ApiResponse(404));
            if (userId != comment.ApplicationUserId)
                return Unauthorized(new ApiResponse(401));
            _unitOfWork.Repository<Comment>().Delete(comment);
            int count = await _unitOfWork.CompleteAsync();
            if (count < 1)
                return BadRequest(new ApiResponse(500, "Failed To Delete Comment"));
            return NoContent();
        }
    }
}
