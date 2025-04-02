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
                    where description.tradable == 1
                    select new SteamAssetItem
                    {
                        Assetid = asset.assetid,
                        Classid = description.classid,
                        Instanceid = description.instanceid,
                        Tradable = description.tradable,
                        MarketName = description.market_name
                    }).ToList();
        }

        public static List<SteamAssetItem> GetAllItems(GetInventoryInfoResponse inventoryResponse)
        {
            if (inventoryResponse == null || inventoryResponse.assets == null || inventoryResponse.descriptions == null)
                return new List<SteamAssetItem>();

            return (from asset in inventoryResponse.assets
                    join description in inventoryResponse.descriptions
                    on new { asset.classid, asset.instanceid } equals new { description.classid, description.instanceid }
                    select new SteamAssetItem
                    {
                        Assetid = asset.assetid,
                        Classid = description.classid,
                        Instanceid = description.instanceid,
                        Tradable = description.tradable,
                        MarketName = description.market_name
                    }).ToList();
        }
    }
}
