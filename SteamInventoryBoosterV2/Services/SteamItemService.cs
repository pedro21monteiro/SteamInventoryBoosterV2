using SteamInventoryBoosterV2.Interfaces.IDao;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Mappings;
using SteamInventoryBoosterV2.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Services
{
    public class SteamItemService : ISteamItemService
    {
        private readonly ISteamItemDao _steamItemDao;
        private readonly IWebScraper _webScraper;
        public SteamItemService(ISteamItemDao steamItemDao, IWebScraper webScraper)
        {
            _steamItemDao = steamItemDao ?? throw new ArgumentNullException(nameof(steamItemDao));
            _webScraper = webScraper ?? throw new ArgumentNullException(nameof(webScraper));
        }

        public async Task<Result<SteamItem>> CreateSteamItemAsync(string link, decimal buyPrice, decimal sellPrice)
        {
            try
            {
                var itemExists = await _steamItemDao.ItemExistsByLinkAsync(link);
                if (itemExists)
                {
                    return Result<SteamItem>.Failure(Error.SteamItemAlreadyExists);
                }

                var colectItemInfoResult = await _webScraper.CollectBasicSteamItemInformation(link);

                if (!colectItemInfoResult.IsSuccess || colectItemInfoResult.Value == null)
                {
                    return Result<SteamItem>.Failure(colectItemInfoResult.Error ?? Error.InternalError);
                }

                SteamItem steamItem = SteamItemMapping.ToEntity(colectItemInfoResult.Value);
                steamItem.BuyPrice = buyPrice;
                steamItem.SellPrice = sellPrice;

                await _steamItemDao.CreateAsync(steamItem);

                return Result<SteamItem>.Success(steamItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to create steam item for {link}: {ex.Message} \n{ex}");
                return Result<SteamItem>.Failure(Error.InternalError);
            }
        }

        public async Task<Result<List<SteamItem>>> GetAllItemsAsync()
        {
            try
            {
                var items = await _steamItemDao.GetAllAsync();

                return Result<List<SteamItem>>.Success(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to get list of steam items: {ex.Message} \n{ex}");
                return Result<List<SteamItem>>.Failure(Error.InternalError);
            }
        }

        public async Task<Result<SteamItem>> EditItemBuySellPricesAsync(int itemId, decimal newBuyPrice, decimal newSellPrice)
        {
            try
            {
                var existingItem = await _steamItemDao.GetSteamItemByIdAsync(itemId);

                if (existingItem == null)
                {
                    return Result<SteamItem>.Failure(Error.SteamItemNotFound);
                }

                existingItem.BuyPrice = newBuyPrice;
                existingItem.SellPrice = newSellPrice;

                await _steamItemDao.UpdateAsync(existingItem);

                return Result<SteamItem>.Success(existingItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to edit steam item buy and sell prices: {ex.Message} \n{ex}");
                return Result<SteamItem>.Failure(Error.InternalError);
            }
        }

        public async Task<Result> DeleteSteamItemAsync(int itemId)
        {
            try
            {
                var existingItem = await _steamItemDao.GetSteamItemByIdAsync(itemId);

                if (existingItem == null)
                {
                    return Result.Failure(Error.SteamItemNotFound);
                }

                await _steamItemDao.DeleteAsync(existingItem);

                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to delete steam item: {ex.Message} \n{ex}");
                return Result<SteamItem>.Failure(Error.InternalError);
            }
        }

    }
}
