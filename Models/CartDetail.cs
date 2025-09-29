using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMagazineStore.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }
        public int MagazineId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required] 
        public double UnitPrice { get; set; }

        public Magazine Magazine { get; set; }
        public Cart Cart { get; set; }

        [NotMapped]
        public object CartDetails { get; internal set; }
    }
}
