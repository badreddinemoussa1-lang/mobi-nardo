namespace MotoMobiNardo.Models;

public class PublicCatalogViewModel
{
    public required BusinessProfile Business { get; init; }
    public bool IsDemo { get; init; }
    public CustomerInquiryInput Inquiry { get; init; } = new();
    public bool InquirySent { get; init; }
    public required IReadOnlyList<Motorcycle> Motorcycles { get; init; }
    public required IReadOnlyList<ExternalMarketListing> MarketListings { get; init; }
    public required IReadOnlyList<string> Brands { get; init; }
    public required IReadOnlyList<int> Years { get; init; }
    public string? Search { get; init; }
    public string? Brand { get; init; }
    public int? Year { get; init; }
    public int? MinCc { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}

public class ExternalMarketListing
{
    public required string Title { get; init; }
    public required string City { get; init; }
    public required string Price { get; init; }
    public decimal? PriceValue { get; init; }
    public required string Details { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public int? YearValue { get; init; }
    public int? MileageKm { get; init; }
    public int? EngineCc { get; init; }
    public required string Published { get; init; }
    public required string Source { get; init; }
    public required string Url { get; init; }
    public string? Note { get; init; }
}
