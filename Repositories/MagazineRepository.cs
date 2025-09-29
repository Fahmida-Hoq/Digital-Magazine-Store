using DigitalMagazineStore.Data;
using DigitalMagazineStore.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalMagazineStore.Repositories
{
    public interface IMagazineRepository
    {
        Task AddMagazine(Magazine magazine);
        Task UpdateMagazine(Magazine magazine);
        Task DeleteMagazine(Magazine magazine);
        Task<Magazine?> GetMagazineById(int id);
        Task<IEnumerable<Magazine>> GetMagazines();
    }

    public class MagazineRepository : IMagazineRepository
    {
        private readonly ApplicationDbContext _context;

        public MagazineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddMagazine(Magazine magazine)
        {
            _context.Magazines.Add(magazine);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMagazine(Magazine magazine)
        {
            _context.Magazines.Update(magazine);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMagazine(Magazine magazine)
        {
            _context.Magazines.Remove(magazine);
            await _context.SaveChangesAsync();
        }

        public async Task<Magazine?> GetMagazineById(int id)
        {
            return await _context.Magazines
                .Include(m => m.Category) // eager load category
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Magazine>> GetMagazines()
        {
            return await _context.Magazines
                .Include(m => m.Category) // eager load category
                .ToListAsync();
        }
    }
}
