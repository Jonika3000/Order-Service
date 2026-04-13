using Grpc.Core;
using OrderService.Application.Contracts;
using OrderService.Application.Services;
using OrderService.Contracts;
using OrderService.Domain.Exceptions;

namespace OrderService.Services;

public sealed class OrderGrpcService(
    OrderApplicationService applicationService,
    ILogger<OrderGrpcService> logger) : OrderGrpc.OrderGrpcBase
{
    public override async Task<CreateOrderReply> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        try
        {
            var order = await applicationService.CreateAsync(
                new CreateOrderCommand(
                    ParseGuid(request.CustomerId, nameof(request.CustomerId)),
                    request.Items.Select(item =>
                        new CreateOrderItemCommand(
                            ParseGuid(item.ProductId, nameof(item.ProductId)),
                            item.ProductName,
                            item.Quantity,
                            Convert.ToDecimal(item.UnitPrice)))
                        .ToArray()),
                context.CancellationToken);

            return new CreateOrderReply { Order = Map(order) };
        }
        catch (Exception exception) when (TryMapException(exception, out var rpcException))
        {
            logger.LogWarning(exception, "CreateOrder failed");
            throw rpcException;
        }
    }

    public override async Task<GetOrderReply> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        try
        {
            var order = await applicationService.GetAsync(
                ParseGuid(request.OrderId, nameof(request.OrderId)),
                context.CancellationToken);

            return new GetOrderReply { Order = Map(order) };
        }
        catch (Exception exception) when (TryMapException(exception, out var rpcException))
        {
            logger.LogWarning(exception, "GetOrder failed");
            throw rpcException;
        }
    }

    public override async Task<PayOrderReply> PayOrder(PayOrderRequest request, ServerCallContext context)
    {
        try
        {
            var order = await applicationService.PayAsync(
                ParseGuid(request.OrderId, nameof(request.OrderId)),
                context.CancellationToken);

            return new PayOrderReply { Order = Map(order) };
        }
        catch (Exception exception) when (TryMapException(exception, out var rpcException))
        {
            logger.LogWarning(exception, "PayOrder failed");
            throw rpcException;
        }
    }

    public override async Task<CancelOrderReply> CancelOrder(CancelOrderRequest request, ServerCallContext context)
    {
        try
        {
            var order = await applicationService.CancelAsync(
                ParseGuid(request.OrderId, nameof(request.OrderId)),
                context.CancellationToken);

            return new CancelOrderReply { Order = Map(order) };
        }
        catch (Exception exception) when (TryMapException(exception, out var rpcException))
        {
            logger.LogWarning(exception, "CancelOrder failed");
            throw rpcException;
        }
    }

    private static OrderModel Map(OrderDto order)
    {
        var model = new OrderModel
        {
            OrderId = order.OrderId.ToString(),
            CustomerId = order.CustomerId.ToString(),
            Status = MapStatus(order.Status),
            TotalAmount = (double)order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc.ToString("O")
        };

        if (order.SubmittedAtUtc is not null)
        {
            model.SubmittedAtUtc = order.SubmittedAtUtc.Value.ToString("O");
        }

        if (order.PaidAtUtc is not null)
        {
            model.PaidAtUtc = order.PaidAtUtc.Value.ToString("O");
        }

        if (order.CancelledAtUtc is not null)
        {
            model.CancelledAtUtc = order.CancelledAtUtc.Value.ToString("O");
        }

        model.Items.AddRange(order.Items.Select(item => new OrderItemModel
        {
            ProductId = item.ProductId.ToString(),
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = (double)item.UnitPrice,
            TotalPrice = (double)item.TotalPrice
        }));

        return model;
    }

    private static OrderStatusModel MapStatus(string status) =>
        status switch
        {
            "Draft" => OrderStatusModel.Draft,
            "Submitted" => OrderStatusModel.Submitted,
            "Paid" => OrderStatusModel.Paid,
            "Cancelled" => OrderStatusModel.Cancelled,
            _ => OrderStatusModel.Unspecified
        };

    private static Guid ParseGuid(string value, string paramName) =>
        Guid.TryParse(value, out var parsed)
            ? parsed
            : throw new DomainException($"{paramName} must be a valid GUID.");

    private static bool TryMapException(Exception exception, out RpcException rpcException)
    {
        rpcException = exception switch
        {
            DomainException domainException => new RpcException(new Status(StatusCode.InvalidArgument, domainException.Message)),
            KeyNotFoundException notFoundException => new RpcException(new Status(StatusCode.NotFound, notFoundException.Message)),
            InvalidOperationException invalidOperationException => new RpcException(new Status(StatusCode.FailedPrecondition, invalidOperationException.Message)),
            _ => new RpcException(new Status(StatusCode.Internal, "Internal server error."))
        };

        return true;
    }
}
