using System.ComponentModel.DataAnnotations;

namespace ContactDemo.WebApp.Models;

public class LoginViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
