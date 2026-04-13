using OrderService.Application.Contracts;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Entities;
using OrderService.Domain.Exceptions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Services;

public sealed class OrderApplicationService(IOrderRepository orderRepository)
{
    public async Task<OrderDto> CreateAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        if (command.Items.Count == 0)
        {
            throw new DomainException("Order must contain at least one item.");
        }

        var order = Order.Create(
            Guid.NewGuid(),
            new CustomerId(command.CustomerId),
            command.Items.Select(item =>
                new OrderItem(
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    new Money(item.UnitPrice))));

        await orderRepository.AddAsync(order, cancellationToken);
        return Map(order);
    }

    public async Task<OrderDto> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(orderId, cancellationToken);
        return Map(order);
    }

    public async Task<OrderDto> PayAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(orderId, cancellationToken);
        order.MarkAsPaid();
        await orderRepository.UpdateAsync(order, cancellationToken);
        return Map(order);
    }

    public async Task<OrderDto> CancelAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(orderId, cancellationToken);
        order.Cancel();
        await orderRepository.UpdateAsync(order, cancellationToken);
        return Map(order);
    }

    private async Task<Order> GetRequiredOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        if (orderId == Guid.Empty)
        {
            throw new DomainException("OrderId cannot be empty.");
        }

        return await orderRepository.GetByIdAsync(orderId, cancellationToken)
               ?? throw new KeyNotFoundException($"Order '{orderId}' was not found.");
    }

    private static OrderDto Map(Order order) =>
        new(
            order.Id,
            order.CustomerId.Value,
            order.Status.ToString(),
            order.Total.Value,
            order.CreatedAtUtc,
            order.SubmittedAtUtc,
            order.PaidAtUtc,
            order.CancelledAtUtc,
            order.Items.Select(item =>
                    new OrderItemDto(
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.UnitPrice.Value,
                        item.TotalPrice.Value))
                .ToArray());
}
