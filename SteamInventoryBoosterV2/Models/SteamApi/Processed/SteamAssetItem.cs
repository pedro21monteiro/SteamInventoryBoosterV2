namespace SteamInventoryBoosterV2.Models.SteamApi.Processed
{
    public class SteamAssetItem
    {
        public string Assetid { get; set; } = String.Empty;
        public string Classid { get; set; } = String.Empty;
        public string Instanceid { get; set; } = String.Empty;
        public int Tradable { get; set; }
        public string MarketName { get; set; } = String.Empty;
    }
}
