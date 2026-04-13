using OrderService.Domain.Exceptions;

namespace OrderService.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal value)
    {
        if (value < 0)
        {
            throw new DomainException("Money value cannot be negative.");
        }

        Value = decimal.Round(value, 2, MidpointRounding.ToEven);
    }

    public decimal Value { get; }

    public static Money Zero => new(0);

    public static Money operator +(Money left, Money right) => new(left.Value + right.Value);

    public static Money operator *(Money money, int multiplier)
    {
        if (multiplier < 0)
        {
            throw new DomainException("Multiplier cannot be negative.");
        }

        return new(money.Value * multiplier);
    }

    public override string ToString() => Value.ToString("0.00");
}
