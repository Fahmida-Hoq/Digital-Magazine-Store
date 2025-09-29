using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMagazineStore.Models
{

    [Table("Category")]

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryType { get; set; }

        public List<Magazine> Magazines { get; set; }

    }
}
