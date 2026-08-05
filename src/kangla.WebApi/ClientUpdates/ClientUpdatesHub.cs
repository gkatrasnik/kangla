using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace kangla.WebApi.ClientUpdates
{
    [Authorize]
    public class ClientUpdatesHub : Hub<IClientUpdatesClient>
    {
    }
}
