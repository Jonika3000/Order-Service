namespace OrderService.Infrastructure.Persistence;

public sealed class OrderRecord
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public int Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public string ItemsJson { get; set; } = "[]";
}
