namespace InOut.OrderAPI.DTOs;

public class CreateOrderDto
{
    public int BuyerId { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = default!;
}
