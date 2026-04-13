using Microsoft.EntityFrameworkCore;
using OrderService.Application.Services;
using OrderService.Domain.Aggregates;

namespace OrderService.Infrastructure.Persistence;

public sealed class PostgresOrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var record = await dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == orderId, cancellationToken);

        return record is null ? null : OrderRecordMapper.ToDomain(record);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await dbContext.Orders.AddAsync(OrderRecordMapper.ToRecord(order), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Update(OrderRecordMapper.ToRecord(order));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
