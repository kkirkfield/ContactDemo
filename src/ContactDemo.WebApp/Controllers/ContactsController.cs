using ContactDemo.Domain.Models;
using ContactDemo.WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactDemo.WebApp.Controllers;

public class ContactsController : Controller
{
    private readonly ILogger<ContactsController> _logger;
    private readonly ContactDemoWebAppContext _context;

    public ContactsController(
        ILogger<ContactsController> logger,
        ContactDemoWebAppContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchString)
    {
        if (_context.Contact is null)
        {
            return Problem("Entity set is null.");
        }

        var filteredContacts = _context.Contact
            .AsNoTracking()
            .Include(c => c.Address)
            .AsQueryable();

        searchString = searchString?.Trim();

        if (!string.IsNullOrEmpty(searchString))
        {
            filteredContacts = filteredContacts
                .Where(c => c.FirstName.Contains(searchString)
                || c.LastName.Contains(searchString)
                || (c.Address != null
                    && (c.Address.Street.Contains(searchString)
                    || c.Address.City.Contains(searchString)
                    || c.Address.State.Contains(searchString)
                    || c.Address.PostalCode.Contains(searchString))));
        }

        return View(await filteredContacts.ToListAsync());
    }

    [HttpGet]
    public Task<IActionResult> Details(int? id) => GetDetailsLikeView(id);

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(
        [Bind(
        nameof(contact.ID),
        nameof(contact.FirstName),
        nameof(contact.LastName),
        nameof(contact.Address))]
        Contact contact)
    {
        if (_context.Contact is null)
        {
            return Problem("Entity set is null.");
        }

        if (!ModelState.IsValid)
        {
            return View(contact);
        }

        _context.Add(contact);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public Task<IActionResult> Edit(int? id) => GetDetailsLikeView(id);

    [HttpPost]
    public async Task<IActionResult> Edit(
        [Bind(
        nameof(contact.ID),
        nameof(contact.FirstName),
        nameof(contact.LastName),
        nameof(contact.Address))]
        Contact contact)
    {
        if (_context.Contact is null)
        {
            return Problem("Entity set is null.");
        }

        if (!ModelState.IsValid)
        {
            return View(contact);
        }

        try
        {
            _context.Update(contact);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!_context.Contact.Any(c => c.ID == contact.ID))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public Task<IActionResult> Delete(int? id) => GetDetailsLikeView(id);

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Contact is null)
        {
            return Problem("Entity set is null.");
        }

        var contact = await _context.Contact.FindAsync(id);

        if (contact is null)
        {
            return NotFound();
        }

        _context.Remove(contact);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> GetDetailsLikeView(int? id)
    {
        if (_context.Contact is null)
        {
            return Problem("Entity set is null.");
        }

        if (id is null)
        {
            return NotFound();
        }

        var contact = await _context.Contact
            .AsNoTracking()
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (contact is null)
        {
            return NotFound();
        }

        return View(contact);
    }
}
