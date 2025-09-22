using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Core;
using Blog.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [Authorize]
    public class FollowsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public FollowsController(IMapper mapper,UserManager<ApplicationUser>userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Follow(FollowDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user  = await _userManager.Users.Include(U=>U.Following).Where(U=>U.Id==userId).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest(new ApiResponse(404));
            var followed =await _userManager.FindByIdAsync(model.Id);
            if (followed is null)
                return BadRequest(new ApiResponse(404, "the user you want to follow is not exist"));
            if (userId == model.Id)
                return BadRequest("You cannot follow yourself");
            if (user.Following.Any(U =>U.FollowedId == model.Id))
                return Ok("You already Follow this user");
           user.Following.Add(new Follow() { FollowedId = model.Id ,FollowerId = userId});
           var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to follow user");
            return Ok("Followed successfully");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> UnFollow(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.Include(U => U.Following).Where(U => U.Id == userId).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest(new ApiResponse(404));
            var followed = await _userManager.FindByIdAsync(id);
            if (followed is null)
                return BadRequest(new ApiResponse(404, "the user you want to Unfollow is not exist"));
            if (userId == id)
                return BadRequest("You cannot unfollow yourself");
            var follow = user.Following.FirstOrDefault(F=>F.FollowedId == id && F.FollowerId == userId);
            if (follow is null)
                return Ok("You already UnFollow this user");
            user.Following.Remove(follow);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to Unfollow user");
            return Ok("UnFollowed successfully");
        }
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FollowToReturnDto>>> GetFollowing()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.Include(U => U.Following).ThenInclude(U=>U.Followed).Where(U => U.Id == userId).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest(new ApiResponse(404));
            
            return Ok(_mapper.Map<IReadOnlyList<FollowToReturnDto>>(user.Following.ToList()));
        }
    }
}
