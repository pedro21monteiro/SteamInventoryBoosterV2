using SteamInventoryBoosterV2.Models.Domain;

namespace SteamInventoryBoosterV2.Interfaces.IDao
{
    public interface ISteamItemDao
    {
        Task<bool> ItemExistsByLinkAsync(string link);
        Task CreateAsync(SteamItem steamItem);
        Task<List<SteamItem>> GetAllAsync();
        Task UpdateAsync(SteamItem item);
        Task<SteamItem?> GetSteamItemByIdAsync(int id);
        Task DeleteAsync(SteamItem item);
    }
}
