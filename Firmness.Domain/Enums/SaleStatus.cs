namespace Firmness.Domain.Enums;
// Possible states of a sale
public enum SaleStatus
{
    Pending = 0,
    Completed = 1,
    Cancelled = 2,
    Processing = 3,
    Refunded = 4
}