using Microsoft.EntityFrameworkCore;
using SteamInventoryBoosterV2.Data;
using SteamInventoryBoosterV2.Interfaces.IDao;
using SteamInventoryBoosterV2.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Dao
{
    public class SteamItemDao : ISteamItemDao
    {
        private readonly AppDbContext _context;
        public SteamItemDao(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ItemExistsByLinkAsync(string link)
        {
            return await _context.SteamItems
                .AnyAsync(s => s.Link == link);
        }

        public async Task CreateAsync(SteamItem steamItem)
        {
            _context.SteamItems.Add(steamItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SteamItem>> GetAllAsync()
        {
            return await _context.SteamItems.ToListAsync();
        }

        public async Task UpdateAsync(SteamItem item)
        {
            _context.SteamItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<SteamItem?> GetSteamItemByIdAsync(int id)
        {
            return await _context.SteamItems.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task DeleteAsync(SteamItem item)
        {
            _context.SteamItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
