using SteamInventoryBoosterV2.Models.SteamApiModels;

namespace SteamInventoryBoosterV2.Models.SteamApi.Response
{
    public class GetInventoryInfoResponse
    {
        public List<Asset> assets { get; set; } = new List<Asset>();
        public List<Description> descriptions { get; set; } = new List<Description>();
        public int total_inventory_count { get; set; }
        public int success { get; set; }
    }
}
