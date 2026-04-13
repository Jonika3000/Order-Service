namespace OrderService.Application.Contracts;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public sealed record OrderDto(
    Guid OrderId,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    DateTime? SubmittedAtUtc,
    DateTime? PaidAtUtc,
    DateTime? CancelledAtUtc,
    IReadOnlyCollection<OrderItemDto> Items);
