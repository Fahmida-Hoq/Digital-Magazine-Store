using DigitalMagazineStore.Models;
using Microsoft.EntityFrameworkCore;
using DigitalMagazineStore.Data;

namespace DigitalMagazineStore.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Stock?> GetStockByMagazineId(int magazineId) =>
            await _context.Stocks.FirstOrDefaultAsync(s => s.MagazineId == magazineId);


        public async Task ManageStock(StockDTO stockToManage)
        {
            // if there is no stock for given book id, then add new record
            // if there is already stock for given book id, update stock's quantity
            var existingStock = await GetStockByMagazineId(stockToManage.MagazineId);

            if (existingStock is null)
            {
                var stock = new Stock { MagazineId = stockToManage.MagazineId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from magazine in _context.Magazines
                                join stock in _context.Stocks
                                    on magazine.Id equals stock.MagazineId into magazine_stock
                                from magazineStock in magazine_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) ||
                                      magazine.Title.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    MagazineId = magazine.Id,
                                    Title = magazine.Title,
                                    Quantity = magazineStock == null ? 0 : magazineStock.Quantity
                                })
                                .ToListAsync();

            return stocks ?? new List<StockDisplayModel>();
        }



    }

}

    public interface IStockRepository
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = ""); 
        Task<Stock?> GetStockByMagazineId(int magazineId);
        Task ManageStock(StockDTO stockToManage);
    }

