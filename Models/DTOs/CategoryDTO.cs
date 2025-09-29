using System.ComponentModel.DataAnnotations;

namespace DigitalMagazineStore.Models.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string CategoryType { get; set; }
    }
}
