using InOut.Shared.Datas;

namespace InOut.Shared.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public int BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItem> OrderItems { get; set; } = default!;
    public Guid IdempotentToken { get; set; }
}