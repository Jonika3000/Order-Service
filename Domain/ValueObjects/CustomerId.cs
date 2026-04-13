using OrderService.Domain.Exceptions;

namespace OrderService.Domain.ValueObjects;

public readonly record struct CustomerId
{
    public CustomerId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("CustomerId cannot be empty.");
        }

        Value = value;
    }

    public Guid Value { get; }

    public override string ToString() => Value.ToString();
}
