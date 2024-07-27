namespace InOut.OrderPublisher.Service.Entities;

public class OrderOutbox
{
    public Guid IdempotentToken { get; set; }
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
}
