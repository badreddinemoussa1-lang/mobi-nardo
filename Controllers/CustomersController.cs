using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

public class CustomersController(MotoMobiDbContext database) : Controller
{
    public IActionResult Index(string? search)
    {
        ViewBag.Search = search;
        var query = database.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.FullName.Contains(search) || c.Phone.Contains(search));
        return View(query.OrderBy(c => c.FullName).ToList());
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);
        database.Customers.Add(customer);
        database.SaveChanges();
        TempData["Success"] = "Le client a été ajouté.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var customer = database.Customers.Find(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        if (!ModelState.IsValid) return View(customer);
        var current = database.Customers.Find(id);
        if (current is null) return NotFound();
        current.FullName = customer.FullName;
        current.Phone = customer.Phone;
        current.Email = customer.Email;
        current.Address = customer.Address;
        database.SaveChanges();
        TempData["Success"] = "Les coordonnées du client ont été mises à jour.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var customer = database.Customers.Find(id);
        if (customer is null) TempData["Error"] = "Client introuvable.";
        else if (database.Sales.Any(s => s.CustomerId == id)) TempData["Error"] = "Ce client est associé à l’historique des ventes et ne peut pas être supprimé.";
        else
        {
            database.Customers.Remove(customer);
            database.SaveChanges();
            TempData["Success"] = "Le client a été supprimé.";
        }
        return RedirectToAction(nameof(Index));
    }
}
