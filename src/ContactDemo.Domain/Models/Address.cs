using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactDemo.Domain.Models;

public class Address
{
    [Key, ForeignKey(nameof(Contact))]
    public int ContactID { get; set; }

    [StringLength(50)]
    public string Street { get; set; } = string.Empty;

    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [StringLength(50)]
    public string State { get; set; } = string.Empty;

    [Display(Name = "Postal code")]
    [StringLength(50)]
    public string PostalCode { get; set; } = string.Empty;

    public Contact? Contact { get; set; }
}
