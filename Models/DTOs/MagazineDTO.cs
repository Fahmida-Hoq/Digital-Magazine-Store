using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DigitalMagazineStore.Models.DTOs
{
    public class MagazineDTO
    {
        

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(60)]

        public string? AuthorName { get; set; }

        [Required]
        public double Price { get; set; }

        public string? CoverImage { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IFormFile? CoverImageFile { get; set; }

        // Dropdown list for Categories (like GenreList in books)
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
