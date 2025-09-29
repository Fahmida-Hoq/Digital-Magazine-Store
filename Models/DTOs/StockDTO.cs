using System.ComponentModel.DataAnnotations;

namespace DigitalMagazineStore.Models.DTOs
{
    public class StockDTO
    {
        public int MagazineId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }
    }
}
