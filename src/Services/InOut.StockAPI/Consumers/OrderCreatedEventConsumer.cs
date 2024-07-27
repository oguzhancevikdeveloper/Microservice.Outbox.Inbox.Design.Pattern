using InOut.Shared.Events;
using InOut.StockAPI.Models.Contexts;
using InOut.StockAPI.Models.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InOut.StockAPI.Consumers;

public class OrderCreatedEventConsumer(StockDbContext _stockDbContext) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var result = await _stockDbContext.OrderInboxes.AnyAsync(x => x.IdempotentToken.Equals(context.Message.IdempotentToken));

        if (!result)
        {
            await _stockDbContext.OrderInboxes.AddAsync(new()
            {

                IdempotentToken = context.Message.IdempotentToken,
                Payload = JsonSerializer.Serialize(context.Message),
                Processed = false,
            });

            await _stockDbContext.SaveChangesAsync();
        }

        List<OrderInbox> orderInbox = await _stockDbContext.OrderInboxes.Where(x => x.Processed.Equals(false)).ToListAsync();

        foreach (var item in orderInbox)
        {
            OrderCreatedEvent? orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(item.Payload);

            Console.WriteLine($"{orderCreatedEvent.OrderId} order id değerine karşılık olan siparişin stok işlemleri başarıyla tamamlanmıştır.");

            item.Processed = true;
            await _stockDbContext.SaveChangesAsync();
        }

    }
}
