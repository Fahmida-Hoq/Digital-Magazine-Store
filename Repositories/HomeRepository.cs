

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace DigitalMagazineStore.Repositories
{
    public class HomeRepository: IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }
        public async Task<IEnumerable<Magazine>> GetMagazines (string sTerm = "", int CategoryID = 0)
        {
            sTerm= sTerm.ToLower();
            IEnumerable<Magazine> magazines = await (from magazine in _db.Magazines
                             join category in _db.Categories
                             on magazine.CategoryID equals category.Id
                             join stock in _db.Stocks
                             on magazine.Id equals stock.MagazineId into magazine_stock
                                from magazineWithStock in magazine_stock.DefaultIfEmpty()
                                                     where string.IsNullOrWhiteSpace(sTerm) || (magazine!= null && magazine.Title.ToLower().StartsWith(sTerm))

                             select new Magazine 
                             {
                                 Id = magazine.Id,
                                 Image = magazine.Image,
                                 AuthorName = magazine.AuthorName,
                                 Title = magazine.Title,
                                 CategoryID = magazine.CategoryID,
                                 Price = magazine.Price,
                                 CategoryType = category.CategoryType,
                                 Quantity = magazineWithStock ==null? 0: magazineWithStock.Quantity
                             }
                            ).ToListAsync();
        if(CategoryID > 0)
            {
                magazines =  magazines.Where(a => a.CategoryID == CategoryID).ToList();

            }
            return magazines;
            
        }
    }
}
