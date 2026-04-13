namespace OrderService.Application.Contracts;

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyCollection<CreateOrderItemCommand> Items);

public sealed record CreateOrderItemCommand(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
