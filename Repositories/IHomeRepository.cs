namespace DigitalMagazineStore
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Magazine>> GetMagazines(string sTerm = "", int CategoryID = 0);
        Task<IEnumerable<Category>> Categories();
    }
}