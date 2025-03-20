using SteamInventoryBoosterV2.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Interfaces.IDao
{
    public interface ISteamItemDao
    {
        Task<bool> ItemExistsByLinkAsync(string link);
        Task CreateAsync(SteamItem steamItem);
        Task<List<SteamItem>> GetAllAsync();
    }
}
