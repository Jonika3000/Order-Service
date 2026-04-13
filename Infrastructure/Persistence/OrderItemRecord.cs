namespace OrderService.Infrastructure.Persistence;

public sealed record OrderItemRecord(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
