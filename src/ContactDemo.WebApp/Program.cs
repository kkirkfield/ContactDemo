using ContactDemo.WebApp.Data;
using ContactDemo.WebApp.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<LoginOptions>()
    .Bind(builder.Configuration.GetSection(nameof(LoginOptions)));

builder.Services.AddDbContext<ContactDemoWebAppContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(nameof(ContactDemoWebAppContext))
        ?? throw new InvalidOperationException($"Connection string '{nameof(ContactDemoWebAppContext)}' not found.")));

builder.Services.AddControllersWithViews(options =>
{
    // Automatically validate antiforgery tokens for unsafe HTTP methods for better security by default.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddScoped<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.LoginPath = "/Home/Index";
    });

builder.Services.AddAuthorization(options =>
{
    // Require authentication by default except during development.
    if (!builder.Environment.IsDevelopment())
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ContactDemoWebAppContext>();
    await SeedData.InitializeAsync(context).ConfigureAwait(false);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

await app.RunAsync().ConfigureAwait(false);
