using System.Text.Json;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Persistence;

public static class OrderRecordMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static OrderRecord ToRecord(Order order)
    {
        var items = order.Items
            .Select(item => new OrderItemRecord(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice.Value))
            .ToArray();

        return new OrderRecord
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = (int)order.Status,
            TotalAmount = order.Total.Value,
            CreatedAtUtc = order.CreatedAtUtc,
            SubmittedAtUtc = order.SubmittedAtUtc,
            PaidAtUtc = order.PaidAtUtc,
            CancelledAtUtc = order.CancelledAtUtc,
            ItemsJson = JsonSerializer.Serialize(items, JsonOptions)
        };
    }

    public static Order ToDomain(OrderRecord record)
    {
        var items = JsonSerializer.Deserialize<List<OrderItemRecord>>(record.ItemsJson, JsonOptions) ?? [];

        return Order.Rehydrate(
            record.Id,
            new CustomerId(record.CustomerId),
            (OrderStatus)record.Status,
            record.CreatedAtUtc,
            record.SubmittedAtUtc,
            record.PaidAtUtc,
            record.CancelledAtUtc,
            items.Select(item => new OrderItem(item.ProductId, item.ProductName, item.Quantity, new Money(item.UnitPrice))));
    }
}
