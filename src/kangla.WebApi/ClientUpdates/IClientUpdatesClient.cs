using kangla.Application.ClientUpdates;

namespace kangla.WebApi.ClientUpdates
{
    public interface IClientUpdatesClient
    {
        Task ClientStateChanged(ClientStateChangedDto change);
    }
}
