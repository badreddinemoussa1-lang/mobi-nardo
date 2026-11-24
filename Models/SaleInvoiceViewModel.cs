namespace MotoMobiNardo.Models;

public class SaleInvoiceViewModel
{
    public required Sale Sale { get; init; }
    public required Motorcycle Motorcycle { get; init; }
    public required Customer Customer { get; init; }
    public required BusinessProfile Business { get; init; }
}

