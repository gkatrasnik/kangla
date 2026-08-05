namespace kangla.Application.ClientUpdates
{
    public class ClientStateChangedDto
    {
        public int? PlantId { get; set; }
        public int? DeviceId { get; set; }
        public IReadOnlyCollection<ClientStateResource> Resources { get; set; } = Array.Empty<ClientStateResource>();
        public DateTime OccurredAtUtc { get; set; }
    }
}
