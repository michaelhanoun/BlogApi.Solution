using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Api.Hubs
{
    [Authorize]
    public class NotificationHub: Hub
    {
    }
}
