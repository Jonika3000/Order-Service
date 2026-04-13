using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderService.Infrastructure.Persistence;

public sealed class OrderRecordConfiguration : IEntityTypeConfiguration<OrderRecord>
{
    public void Configure(EntityTypeBuilder<OrderRecord> builder)
    {
        builder.ToTable("order_service");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasColumnName("id");

        builder.Property(order => order.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(order => order.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(order => order.SubmittedAtUtc)
            .HasColumnName("submitted_at_utc");

        builder.Property(order => order.PaidAtUtc)
            .HasColumnName("paid_at_utc");

        builder.Property(order => order.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(order => order.ItemsJson)
            .HasColumnName("items_json")
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
