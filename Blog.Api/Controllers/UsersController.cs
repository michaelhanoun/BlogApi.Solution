using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Api.Dtos;
using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace Blog.Api.Controllers
{
    public class UsersController :BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser>userManager,IMapper mapper,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }
        [Authorize(Roles ="Admin")]
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserToReturnDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(_mapper.Map<IReadOnlyList<UserToReturnDto>>(users));
        }
        [ProducesResponseType(typeof(UserToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        [Authorize(Policy = "SameUserOrAdminForUserController")]
        [Cached(600)]

        [HttpGet("{id}")]
        public async Task<ActionResult<UserToReturnDto>> GetUser(string id)
        {  
            var user = await _userManager.FindByIdAsync(id);

            if(user is null)
            return BadRequest(new ApiResponse(404));
            return Ok(_mapper.Map<UserToReturnDto>(user));
        }
        [Authorize(Policy = "SameUserOrAdminForUserController")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserToReturnDto>> UpdateUser(string id,UpdateDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound(new ApiResponse(404));
            if (!string.IsNullOrWhiteSpace(updateDto.UserName))
                user.UserName = updateDto.UserName;
            if (!string.IsNullOrWhiteSpace(updateDto.Bio))
                user.Bio = updateDto.Bio;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400,string.Join(',',result.Errors.Select(E=>E.Description))));

            return Ok(_mapper.Map<UserToReturnDto>(user));
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound(new ApiResponse(404));
           var result = await _userManager.DeleteAsync(user);
            if(!result.Succeeded)
                return BadRequest(new ApiResponse(400, string.Join(',', result.Errors.Select(E => E.Description))));
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/role")]
        public async Task<ActionResult<UserRoleDto>> AddRoleToUser(string id,UpdateUserRoleDto updateUserRoleDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user is null)
                return NotFound(new ApiResponse(404)); 
            var currentRoles = await _userManager.GetRolesAsync(user);
            if(currentRoles.Any())
            {
                var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if(!result.Succeeded)
                    return NotFound(new ApiResponse(500, "Failed to remove  role."));

            }
            if (string.IsNullOrWhiteSpace(updateUserRoleDto.Role))
            {
                return Ok(new UserRoleDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = null
                });
            }
            var isRoleExist = await _roleManager.RoleExistsAsync(updateUserRoleDto.Role);
            if(!isRoleExist)
                return NotFound(new ApiResponse(404));
            var resultInAddingRole = await _userManager.AddToRoleAsync(user, updateUserRoleDto.Role);
            if(!resultInAddingRole.Succeeded)
                return NotFound(new ApiResponse(500,"Failed to assign new role."));

           return Ok(new UserRoleDto(){Id = user.Id,UserName = user.UserName,Email = user.Email,Role = updateUserRoleDto.Role});
        }
    }
}
