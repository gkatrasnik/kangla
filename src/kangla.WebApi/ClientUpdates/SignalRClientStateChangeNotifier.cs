using kangla.Application.ClientUpdates;
using Microsoft.AspNetCore.SignalR;

namespace kangla.WebApi.ClientUpdates
{
    public class SignalRClientStateChangeNotifier : IClientStateChangeNotifier
    {
        private readonly IHubContext<ClientUpdatesHub, IClientUpdatesClient> _hubContext;
        private readonly ILogger<SignalRClientStateChangeNotifier> _logger;

        public SignalRClientStateChangeNotifier(
            IHubContext<ClientUpdatesHub, IClientUpdatesClient> hubContext,
            ILogger<SignalRClientStateChangeNotifier> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAsync(string userId, ClientStateChangedDto change)
        {
            try
            {
                await _hubContext.Clients.User(userId).ClientStateChanged(change);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Could not deliver a client state change to user {UserId}.",
                    userId);
            }
        }
    }
}
