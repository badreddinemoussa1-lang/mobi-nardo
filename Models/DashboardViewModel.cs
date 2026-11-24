namespace MotoMobiNardo.Models;

public class DashboardViewModel
{
    public bool IsDemo { get; set; }
    public int SelectedYear { get; set; }
    public string? SelectedBrand { get; set; }
    public List<int> YearOptions { get; set; } = [];
    public List<string> BrandOptions { get; set; } = [];
    public int MotorcycleCount { get; set; }
    public int UnitsInStock { get; set; }
    public int CustomerCount { get; set; }
    public int OpenInquiryCount { get; set; }
    public int ReservedInquiryCount { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal ThisWeekRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal InventoryRetailValue { get; set; }
    public decimal Revenue { get; set; }
    public int UnitsSold { get; set; }
    public int ActiveSaleCount { get; set; }
    public decimal AverageSale { get; set; }
    public string? BestSeller { get; set; }
    public int BestSellerUnits { get; set; }
    public List<DashboardMonthMetric> MonthlyRevenue { get; set; } = [];
    public List<DashboardBrandMetric> BrandRevenue { get; set; } = [];
    public List<DashboardChannelMetric> ChannelSales { get; set; } = [];
    public List<DashboardRecentSale> RecentSales { get; set; } = [];
    public List<Motorcycle> LowStock { get; set; } = [];
}

public class DashboardMonthMetric
{
    public required string Label { get; init; }
    public decimal Revenue { get; init; }
    public int Units { get; init; }
    public double HeightPercent { get; init; }
}

public class DashboardBrandMetric
{
    public required string Brand { get; init; }
    public decimal Revenue { get; init; }
    public int Units { get; init; }
    public double WidthPercent { get; init; }
}

public class DashboardChannelMetric
{
    public required string Channel { get; init; }
    public decimal Revenue { get; init; }
    public int Sales { get; init; }
    public int Units { get; init; }
    public double WidthPercent { get; init; }
}

public class DashboardRecentSale
{
    public required Sale Sale { get; init; }
    public required string Motorcycle { get; init; }
    public required string Customer { get; init; }
}
