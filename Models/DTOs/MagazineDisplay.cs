
namespace DigitalMagazineStore.Models.DTOs
{
    public class MagazineDisplay
    {
        public IEnumerable<Magazine> Magazines { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string STerm { get; set; } = "";

        public int CategoryID { get; set; } = 0;

        
    }
}
