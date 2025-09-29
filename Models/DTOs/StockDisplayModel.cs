namespace DigitalMagazineStore.Models.DTOs
{
    public class StockDisplayModel
    {
        public int Id { get; set; }
        public int MagazineId { get; set; }
        public int Quantity { get; set; }
        public string? Title { get; set; }
    }

}
