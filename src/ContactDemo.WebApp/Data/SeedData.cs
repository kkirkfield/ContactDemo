using Bogus;
using ContactDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactDemo.WebApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ContactDemoWebAppContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Contact.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var addressFaker = new Faker<Address>()
            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.State())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode());

        var contactFaker = new Faker<Contact>()
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Address, f => addressFaker.Generate());

        context.Contact.AddRange(contactFaker.GenerateLazy(20));
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
