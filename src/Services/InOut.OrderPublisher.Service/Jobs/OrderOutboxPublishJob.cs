using InOut.OrderPublisher.Service.Entities;
using InOut.Shared.Events;
using MassTransit;
using Quartz;
using System.Text.Json;

namespace InOut.OrderPublisher.Service.Jobs;

public class OrderOutboxPublishJob(IPublishEndpoint _publishEndpoint) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {

        if (OrderOutboxSingletonDatabase.DataReaderState)
        {
            OrderOutboxSingletonDatabase.DataReaderBusy();

            List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM ORDEROUTBOXES WHERE PROCESSEDDATE IS NULL ORDER BY OCCUREDON ASC")).ToList();


            foreach (var orderOutbox in orderOutboxes)
            {
                if (orderOutbox.Type == nameof(OrderCreatedEvent))
                {
                    OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);

                    if (orderCreatedEvent != null)
                    {
                        await _publishEndpoint.Publish(orderCreatedEvent);
                        OrderOutboxSingletonDatabase.ExecuteAsync($"UPDATE ORDEROUTBOXES SET PROCESSEDDATE = GETDATE() WHERE IdempotentToken = '{orderOutbox.IdempotentToken}'");

                    }
                }
            }

            OrderOutboxSingletonDatabase.DataReaderReady();
            await Console.Out.WriteLineAsync("Order outbox table checked!");
        }
    }
}
