namespace DigitalMagazineStore.Models.DTOs
{
    public class OrderDetailModelDTO
    {
     public string DivId { get; set; }
    public required IEnumerable<OrderDetail> OrderDetail { get; set; }
    
    }
}
