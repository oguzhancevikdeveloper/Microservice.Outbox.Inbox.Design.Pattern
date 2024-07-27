namespace InOut.OrderAPI.DTOs;

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}
