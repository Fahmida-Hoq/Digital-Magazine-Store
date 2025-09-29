using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMagazineStore.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public int MagazineId { get; set; }

        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public Order Order { get; set; }
        
        public Magazine Magazine{ get; set; }
    }
}
