using System.ComponentModel.DataAnnotations;
using ToursAndTravelsManagement.Attributes;

namespace ToursAndTravelsManagement.Models;

public class Tour
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [FutureDate]
    public DateTime EndDate { get; set; }
}