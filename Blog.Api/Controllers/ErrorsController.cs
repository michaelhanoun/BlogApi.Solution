using Blog.Api.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
       
        public IActionResult Error(int code)
        {
            return code switch
            {
                401 => Unauthorized(new ApiResponse(401)),
                404 => BadRequest(new ApiResponse(404)),
                _ => StatusCode(code)
            };
        }
    }
}
