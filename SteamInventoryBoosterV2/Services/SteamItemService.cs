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
                Console.WriteLine($"Internal error while trying to create steam item for {link}: {ex.Message}");
                return Result<SteamItem>.Failure(Error.InternalError);
            }
        }

    }
}
