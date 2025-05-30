using SteamInventoryBoosterV2.Models.SteamApi.Processed;
using SteamInventoryBoosterV2.Models.SteamApi.Response;

namespace SteamInventoryBoosterV2.Services
{
    public static class InventoryHelper
    {
        public static List<SteamAssetItem> GetTradableItems(GetInventoryInfoResponse inventoryResponse)
        {
            if (inventoryResponse == null || inventoryResponse.assets == null || inventoryResponse.descriptions == null)
                return new List<SteamAssetItem>();

            return (from asset in inventoryResponse.assets
                    join description in inventoryResponse.descriptions
                    on new { asset.classid, asset.instanceid } equals new { description.classid, description.instanceid }
                    into joinedDescriptions
                    let firstTradableDescription = joinedDescriptions
                        .Where(d => d.tradable == 1)
                        .FirstOrDefault()
                    where firstTradableDescription != null
                    select new SteamAssetItem
                    {
                        Assetid = asset.assetid,
                        Classid = asset.classid,
                        Instanceid = asset.instanceid,
                        Tradable = firstTradableDescription.tradable,
                        MarketName = firstTradableDescription.market_name
                    }).ToList();
        }

        public static List<SteamAssetItem> GetAllItems(GetInventoryInfoResponse inventoryResponse)
        {
            if (inventoryResponse == null || inventoryResponse.assets == null || inventoryResponse.descriptions == null)
                return new List<SteamAssetItem>();

            return (from asset in inventoryResponse.assets
                    join description in inventoryResponse.descriptions
                    on new { asset.classid, asset.instanceid } equals new { description.classid, description.instanceid }
                    into joinedDescriptions
                    let firstTradableDescription = joinedDescriptions
                        .FirstOrDefault()
                    where firstTradableDescription != null
                    select new SteamAssetItem
                    {
                        Assetid = asset.assetid,
                        Classid = asset.classid,
                        Instanceid = asset.instanceid,
                        Tradable = firstTradableDescription.tradable,
                        MarketName = firstTradableDescription.market_name
                    }).ToList();
        }
    }
}
