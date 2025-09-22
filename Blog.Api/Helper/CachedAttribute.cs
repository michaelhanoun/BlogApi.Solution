using System.Text;
using Blog.Core.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Api.Helper
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var response = await responseCacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    StatusCode = 200,
                    ContentType = "application/json"
                };
                context.Result = result;
                return;
            }
              var actionExecutedContext = await next.Invoke();

            if (actionExecutedContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey,okObjectResult.Value,TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(request.Path);
            var sortedQuery = request.Query.OrderBy(x => x.Key);
            foreach (var (key,value) in sortedQuery)
            {
                stringBuilder.Append($"|{key}-{value}");
            }
            return stringBuilder.ToString();
        }
    }
}
