using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;
using System.Globalization;

namespace MotoMobiNardo.Controllers;

public class HomeController(MotoMobiDbContext database, IConfiguration configuration) : Controller
{
    public IActionResult Index(int? year, string? brand)
    {
        var motorcycles = database.Motorcycles.AsNoTracking().ToList();
        var years = database.Sales.AsNoTracking().Where(s => !s.IsCancelled).Select(s => s.SoldAt.Year).Distinct().ToList();
        var currentYear = DateTime.Today.Year;
        if (!years.Contains(currentYear)) years.Add(currentYear);
        years = years.OrderByDescending(y => y).ToList();
        var selectedYear = year.HasValue && years.Contains(year.Value) ? year.Value : currentYear;

        var salesForYear = (from sale in database.Sales.AsNoTracking()
                            join motorcycle in database.Motorcycles.AsNoTracking() on sale.MotorcycleId equals motorcycle.Id
                            join customer in database.Customers.AsNoTracking() on sale.CustomerId equals customer.Id
                            where !sale.IsCancelled && sale.SoldAt.Year == selectedYear
                            select new
                            {
                                Sale = sale,
                                Brand = motorcycle.Brand,
                                Motorcycle = motorcycle.Brand + " " + motorcycle.Model,
                                Customer = customer.FullName
                            }).ToList();

        var brands = motorcycles.Select(m => m.Brand)
            .Concat(salesForYear.Select(s => s.Brand))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(b => b)
            .ToList();
        var selectedBrand = brands.FirstOrDefault(b => string.Equals(b, brand, StringComparison.OrdinalIgnoreCase));
        var filteredSales = salesForYear
            .Where(s => selectedBrand is null || string.Equals(s.Brand, selectedBrand, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var maxMonthlyRevenue = Enumerable.Range(1, 12)
            .Select(month => filteredSales.Where(s => s.Sale.SoldAt.Month == month).Sum(s => s.Sale.Total))
            .DefaultIfEmpty(0m).Max();
        var monthlyRevenue = Enumerable.Range(1, 12).Select(month =>
        {
            var monthSales = filteredSales.Where(s => s.Sale.SoldAt.Month == month).ToList();
            var revenue = monthSales.Sum(s => s.Sale.Total);
            return new DashboardMonthMetric
            {
                Label = new DateTime(selectedYear, month, 1).ToString("MMM", CultureInfo.GetCultureInfo("fr-MA")),
                Revenue = revenue,
                Units = monthSales.Sum(s => s.Sale.Quantity),
                HeightPercent = maxMonthlyRevenue == 0 ? 0 : Math.Max(4, (double)(revenue / maxMonthlyRevenue * 100m))
            };
        }).ToList();

        var brandGroups = filteredSales.GroupBy(s => s.Brand)
            .Select(group => new { Brand = group.Key, Revenue = group.Sum(s => s.Sale.Total), Units = group.Sum(s => s.Sale.Quantity) })
            .OrderByDescending(group => group.Revenue)
            .Take(6)
            .ToList();
        var maxBrandRevenue = brandGroups.Select(group => group.Revenue).DefaultIfEmpty(0m).Max();
        var brandRevenue = brandGroups.Select(group => new DashboardBrandMetric
        {
            Brand = group.Brand,
            Revenue = group.Revenue,
            Units = group.Units,
            WidthPercent = maxBrandRevenue == 0 ? 0 : Math.Max(3, (double)(group.Revenue / maxBrandRevenue * 100m))
        }).ToList();

        var channelGroups = filteredSales.GroupBy(s => string.IsNullOrWhiteSpace(s.Sale.Channel) ? SalesChannels.Unspecified : s.Sale.Channel)
            .Select(group => new { Channel = group.Key, Revenue = group.Sum(s => s.Sale.Total), Sales = group.Count(), Units = group.Sum(s => s.Sale.Quantity) })
            .OrderByDescending(group => group.Sales)
            .ToList();
        var maxChannelSales = channelGroups.Select(group => group.Sales).DefaultIfEmpty(0).Max();
        var channelSales = channelGroups.Select(group => new DashboardChannelMetric
        {
            Channel = group.Channel,
            Revenue = group.Revenue,
            Sales = group.Sales,
            Units = group.Units,
            WidthPercent = maxChannelSales == 0 ? 0 : Math.Max(3, (double)group.Sales / maxChannelSales * 100d)
        }).ToList();

        var bestSeller = filteredSales.GroupBy(s => s.Motorcycle)
            .Select(group => new { Motorcycle = group.Key, Units = group.Sum(s => s.Sale.Quantity) })
            .OrderByDescending(group => group.Units)
            .ThenBy(group => group.Motorcycle)
            .FirstOrDefault();
        var revenueTotal = filteredSales.Sum(s => s.Sale.Total);
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var periodSales = database.Sales.AsNoTracking()
            .Where(s => !s.IsCancelled && s.SoldAt >= weekStart && s.SoldAt < today.AddDays(1))
            .Select(s => new { s.SoldAt, s.UnitPrice, s.Quantity, s.Discount })
            .ToList();
        var model = new DashboardViewModel
        {
            IsDemo = string.Equals(configuration["MOBI_NARDO_DEMO_MODE"], "true", StringComparison.OrdinalIgnoreCase),
            MotorcycleCount = motorcycles.Count,
            UnitsInStock = motorcycles.Sum(m => m.Stock),
            CustomerCount = database.Customers.Count(),
            OpenInquiryCount = database.CustomerInquiries.Count(i => i.Status != CustomerInquiryStatuses.Won && i.Status != CustomerInquiryStatuses.Lost),
            ReservedInquiryCount = database.CustomerInquiries.Count(i => i.Status == CustomerInquiryStatuses.Reserved),
            TodayRevenue = periodSales.Where(s => s.SoldAt >= today).Sum(s => Math.Max(0m, s.UnitPrice * s.Quantity - s.Discount)),
            ThisWeekRevenue = periodSales.Sum(s => Math.Max(0m, s.UnitPrice * s.Quantity - s.Discount)),
            ThisMonthRevenue = periodSales.Where(s => s.SoldAt >= monthStart).Sum(s => Math.Max(0m, s.UnitPrice * s.Quantity - s.Discount)),
            InventoryRetailValue = motorcycles.Sum(m => m.Stock * m.Price),
            SelectedYear = selectedYear,
            SelectedBrand = selectedBrand,
            YearOptions = years,
            BrandOptions = brands,
            Revenue = revenueTotal,
            UnitsSold = filteredSales.Sum(s => s.Sale.Quantity),
            ActiveSaleCount = filteredSales.Count,
            AverageSale = filteredSales.Count == 0 ? 0 : revenueTotal / filteredSales.Count,
            BestSeller = bestSeller?.Motorcycle,
            BestSellerUnits = bestSeller?.Units ?? 0,
            MonthlyRevenue = monthlyRevenue,
            BrandRevenue = brandRevenue,
            ChannelSales = channelSales,
            RecentSales = filteredSales.OrderByDescending(s => s.Sale.SoldAt).ThenByDescending(s => s.Sale.Id)
                .Take(6).Select(s => new DashboardRecentSale { Sale = s.Sale, Motorcycle = s.Motorcycle, Customer = s.Customer }).ToList(),
            LowStock = motorcycles.Where(m => m.Stock <= 2).OrderBy(m => m.Stock).ThenBy(m => m.Brand).Take(5).ToList()
        };
        return View(model);
    }
}
