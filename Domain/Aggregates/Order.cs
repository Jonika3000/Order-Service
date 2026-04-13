using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Aggregates;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order(Guid id, CustomerId customerId)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("OrderId cannot be empty.");
        }

        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public CustomerId CustomerId { get; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money Total => _items.Aggregate(Money.Zero, (current, item) => current + item.TotalPrice);

    public static Order Create(Guid id, CustomerId customerId, IEnumerable<OrderItem> items)
    {
        var order = new Order(id, customerId);

        foreach (var item in items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        order.Submit();
        return order;
    }

    public static Order Rehydrate(
        Guid id,
        CustomerId customerId,
        OrderStatus status,
        DateTime createdAtUtc,
        DateTime? submittedAtUtc,
        DateTime? paidAtUtc,
        DateTime? cancelledAtUtc,
        IEnumerable<OrderItem> items)
    {
        var order = new Order(id, customerId)
        {
            Status = status,
            SubmittedAtUtc = submittedAtUtc,
            PaidAtUtc = paidAtUtc,
            CancelledAtUtc = cancelledAtUtc
        };

        order.OverrideCreatedAt(createdAtUtc);

        foreach (var item in items)
        {
            order._items.Add(item);
        }

        return order;
    }

    public void AddItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        EnsureMutable();

        var existingItem = _items.SingleOrDefault(item => item.ProductId == productId);
        if (existingItem is null)
        {
            _items.Add(new OrderItem(productId, productName, quantity, unitPrice));
            return;
        }

        existingItem.IncreaseQuantity(quantity);
    }

    public void Submit()
    {
        EnsureMutable();

        if (_items.Count == 0)
        {
            throw new DomainException("Order must contain at least one item.");
        }

        Status = OrderStatus.Submitted;
        SubmittedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new DomainException("Cancelled order cannot be paid.");
        }

        if (Status == OrderStatus.Paid)
        {
            throw new DomainException("Order is already paid.");
        }

        if (_items.Count == 0)
        {
            throw new DomainException("Order without items cannot be paid.");
        }

        Status = OrderStatus.Paid;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
        {
            throw new DomainException("Paid order cannot be cancelled.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            throw new DomainException("Order is already cancelled.");
        }

        Status = OrderStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }

    private void EnsureMutable()
    {
        if (Status is OrderStatus.Paid or OrderStatus.Cancelled)
        {
            throw new DomainException($"Order in status {Status} cannot be changed.");
        }
    }

    private void OverrideCreatedAt(DateTime createdAtUtc)
    {
        if (createdAtUtc == default)
        {
            throw new DomainException("CreatedAtUtc must be specified.");
        }

        CreatedAtUtc = createdAtUtc;
    }
}
