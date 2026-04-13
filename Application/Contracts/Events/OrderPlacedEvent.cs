using OrderService.Domain.Enums;

namespace OrderService.Application.Contracts.Events;

public sealed record OrderPlacedEvent(
    Guid EventId,
    Guid OrderId,
    Guid CustomerId,
    IReadOnlyCollection<OrderPlacedItemEvent> Items,
    decimal TotalAmount,
    string DeliveryAddress,
    DateTime OccurredAt);

public sealed record OrderPlacedEventV2(
    Guid EventId,
    Guid OrderId,
    Guid CustomerId,
    IReadOnlyCollection<OrderPlacedItemEvent> Items,
    decimal TotalAmount,
    string? DeliveryAddress,
    string TargetAddress,
    LoyaltyTier LoyaltyTier,
    DateTime OccurredAt);

public sealed record OrderPlacedItemEvent(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
