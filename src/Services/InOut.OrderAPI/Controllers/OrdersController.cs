using InOut.OrderAPI.DTOs;
using InOut.OrderAPI.Models.Contexts;
using InOut.OrderAPI.Models.Entities;
using InOut.Shared.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InOut.OrderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext _orderDbContext, ISendEndpointProvider _sendEndpointProvider) : ControllerBase
{
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        Order order = new()
        {
            BuyerId = createOrderDto.BuyerId,
            OrderItems = createOrderDto.OrderItems.Select(x => new OrderItem
            {
                Count = x.Count,
                Price = x.Price,
                ProductId = x.ProductId,
            }).ToList(),
            CreatedDate = DateTime.Now,
            TotalPrice = createOrderDto.OrderItems.Sum(x => x.Price * x.Count)
        };

        await _orderDbContext.Orders.AddAsync(order);
        await _orderDbContext.SaveChangesAsync();

        var idempotentToken = Guid.NewGuid();

        OrderCreatedEvent orderCreatedEvent = new()
        {
            BuyerId = order.BuyerId,
            IdempotentToken = idempotentToken,
            OrderId = order.Id,
            OrderItems = order.OrderItems.Select(x => new Shared.Datas.OrderItem
            {
                Count = x.Count,
                Price = x.Price,
                ProductId = x.ProductId
            }).ToList(),
            TotalPrice = order.TotalPrice
        };

        #region Outbox Pattern Olmaksýzýn!
        //var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEvent}"));
        //await sendEndpoint.Send<OrderCreatedEvent>(orderCreatedEvent);
        #endregion

        #region

        OrderOutbox orderOutbox = new()
        {
            IdempotentToken = idempotentToken,
            OccuredOn = DateTime.Now,
            ProcessedDate = null,
            Type = nameof(OrderCreatedEvent),
            Payload = JsonSerializer.Serialize(orderCreatedEvent),
        };

        await _orderDbContext.OrderOutboxes.AddAsync(orderOutbox);
        await _orderDbContext.SaveChangesAsync();

        return Ok();

        #endregion

    }
}
