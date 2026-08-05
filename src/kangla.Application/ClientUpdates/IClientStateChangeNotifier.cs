namespace kangla.Application.ClientUpdates
{
    public interface IClientStateChangeNotifier
    {
        Task NotifyAsync(string userId, ClientStateChangedDto change);
    }
}
