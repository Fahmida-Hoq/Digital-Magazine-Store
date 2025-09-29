using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMagazineStore.Models
{
    [Table ("Magazine")]
    public class Magazine
    {
        internal readonly object MagazineId;
        internal int Quantity;
        internal int CategoryId;

        public int Id { get; set; }

        [Required]
        [MaxLength (50)]

        public string? Title { get; set; }

        [Required]
        [MaxLength(50)]

        public string? AuthorName { get; set; }

        public double Price { get; set; }
        public string? Image  { get; set; }
        [Required]
        public int CategoryID { get; set; }

        public  Category Category { get; set; }

        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }

        public Stock Stock { get; set; }
        [NotMapped]
        public string CategoryType { get; set; }   

    }
}
