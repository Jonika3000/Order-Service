using OrderService.Domain.Exceptions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public sealed class OrderItem
{
    public OrderItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("ProductId cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("ProductName is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        ProductId = productId;
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; }
    public Money TotalPrice => UnitPrice * Quantity;

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Quantity increment must be greater than zero.");
        }

        Quantity += quantity;
    }
}
