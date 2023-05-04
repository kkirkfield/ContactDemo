using ContactDemo.WebApp.Models;
using ContactDemo.WebApp.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace ContactDemo.WebApp.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly LoginOptions _options;
    private readonly ILogger<HomeController> _logger;
    private readonly IPasswordHasher<IdentityUser> _passwordHasher;

    private string DefaultReturnUrl => Url.Content("~/Contacts/Index");

    public HomeController(
        IOptions<LoginOptions> options,
        ILogger<HomeController> logger,
        IPasswordHasher<IdentityUser> passwordHasher)
    {
        _options = options.Value;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl)
    {
        _logger.LogDebug("GET request to login page with ReturnUrl=`{ReturnUrl}`.", returnUrl);

        returnUrl ??= DefaultReturnUrl;

        if (EnsurePasswordIsSetAndUserIsLoggedOut(returnUrl) is IActionResult redirectResult)
        {
            return redirectResult;
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost, ActionName("Index")]
    public async Task<IActionResult> Login(
        [Bind(nameof(LoginViewModel.Password), nameof(LoginViewModel.ReturnUrl))]
        LoginViewModel model)
    {
        _logger.LogDebug("POST request to login page with ReturnUrl=`{ReturnUrl}`.", model.ReturnUrl);

        model.ReturnUrl ??= DefaultReturnUrl;

        if (EnsurePasswordIsSetAndUserIsLoggedOut(model.ReturnUrl) is IActionResult redirectResult)
        {
            return redirectResult;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(new IdentityUser(), _options.HashedPassword, model.Password);

        if (passwordVerificationResult is PasswordVerificationResult.Failed)
        {
            _logger.LogError("User failed to log in with demo password.");
            ModelState.AddModelError(nameof(model.Password), "Invalid demo password.");
            return View(model);
        }

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.AuthenticationMethod, "Demo Password")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return LocalRedirect(model.ReturnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Hash() => View();

    [HttpPost]
    public IActionResult Hash(string password)
    {
        var hashedPassword = _passwordHasher.HashPassword(new IdentityUser(), password);
        ViewData["HashedPassword"] = hashedPassword;
        return View();
    }

    [HttpGet]
    public IActionResult Privacy() => View();

    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? id)
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("An error occurred. RequestId=`{RequestId}`.", requestId);

        var message = id switch
        {
            404 => "The page or resource you are looking for could not be found.",
            _ => "An error occurred while processing your request."
        };

        return View(new ErrorViewModel { RequestId = requestId, Message = message });
    }

    private IActionResult? EnsurePasswordIsSetAndUserIsLoggedOut(string returnUrl)
    {
        if (string.IsNullOrEmpty(_options.HashedPassword))
        {
            _logger.LogError("Demo password is not set. Redirecting to demo password setup page.");
            return RedirectToAction(nameof(Hash));
        }

        if (User.Identity?.IsAuthenticated is true)
        {
            _logger.LogDebug("User is already authenticated. Redirecting to ReturnUrl=`{ReturnUrl}`.", returnUrl);
            return LocalRedirect(returnUrl);
        }

        return null;
    }
}
