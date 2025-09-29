using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMagazineStore.Models
{
    [Table("Stock")]
    public class Stock
    {
        internal object Category;

        public int Id { get; set; }
        public int MagazineId { get; set; }
        public int Quantity { get; set; }

        public Magazine? Magazine { get; set; }
    }
}
