using System.ComponentModel.DataAnnotations;

namespace ContactDemo.Domain.Models;

public class Contact
{
    public int ID { get; set; }

    [Display(Name = "First name")]
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last name")]
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    public string FullName => string.Join(" ", FirstName, LastName);

    public Address? Address { get; set; }
}
