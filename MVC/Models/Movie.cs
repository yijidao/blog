using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [StringLength(30)]
    public string Genre { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [DataType(DataType.Currency)]
    [Range(1, 100)]
    public decimal Price { get; set; }

    [Required, StringLength(5)]
    public string Rating { get; set; } = string.Empty;
}