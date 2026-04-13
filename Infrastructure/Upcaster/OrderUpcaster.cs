using OrderService.Application.Contracts.Events;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.UpCaster;

public class OrderUpcaster
{
    public OrderPlacedEventV2 UpcastOrderPlacedEventToV2(OrderPlacedEvent old)
    {
        return new OrderPlacedEventV2(
            EventId:            old.EventId,
            OrderId:            old.OrderId,
            CustomerId:         old.CustomerId,
            Items:              old.Items,
            TotalAmount:        old.TotalAmount,
            DeliveryAddress:    null,
            DeliveryAddressNew: old.DeliveryAddress,
            LoyaltyTier:        LoyaltyTier.Standard,
            OccurredAt:         old.OccurredAt
        );
    }
}