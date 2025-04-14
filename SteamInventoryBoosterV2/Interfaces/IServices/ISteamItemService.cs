using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Utilities;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface ISteamItemService
    {
        Task<Result<SteamItem>> CreateSteamItemAsync(string link, decimal buyPrice, decimal sellPrice);
        Task<Result<List<SteamItem>>> GetAllItemsAsync();
        Task<Result<SteamItem>> EditItemBuySellPricesAsync(string classId, decimal newBuyPrice, decimal newSellPrice);
        Task<Result> DeleteSteamItemAsync(string classId);
        Task<Result<SteamItem>> UpdateSteamItemInfoAsync(string classId);
        Task<Result<SteamItem>> ChangeVisibilityAsync(string classId);
        Task<Result<SteamItem>> GetSteamItemByNameAsync(string name);
        Task<Result<SteamItem>> GetSteamItemByClassIdAsync(string classId);
    }
}
