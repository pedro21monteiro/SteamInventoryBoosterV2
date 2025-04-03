using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Utilities;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface ISteamItemService
    {
        Task<Result<SteamItem>> CreateSteamItemAsync(string link, decimal buyPrice, decimal sellPrice);
        Task<Result<List<SteamItem>>> GetAllItemsAsync();
        Task<Result<SteamItem>> EditItemBuySellPricesAsync(int itemId, decimal newBuyPrice, decimal newSellPrice);
        Task<Result> DeleteSteamItemAsync(int itemId);
        Task<Result<SteamItem>> UpdateSteamItemInfoAsync(int itemId);
        Task<Result<SteamItem>> ChangeVisibilityAsync(int itemId);
        Task<Result<SteamItem>> GetSteamItemByNameAsync(string name);
    }
}
