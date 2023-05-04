using ContactDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactDemo.WebApp.Data;

public class ContactDemoWebAppContext : DbContext
{
    public ContactDemoWebAppContext(DbContextOptions<ContactDemoWebAppContext> options)
        : base(options) { }

    public DbSet<Contact> Contact { get; set; } = default!;
}
