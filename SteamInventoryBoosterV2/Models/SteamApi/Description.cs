namespace SteamInventoryBoosterV2.Models.SteamApiModels
{
    public class Description
    {
        public int appid { get; set; }
        public string classid { get; set; } = String.Empty;
        public string instanceid { get; set; } = String.Empty;
        public string icon_url { get; set; } = String.Empty;
        public int tradable { get; set; }
        public string name { get; set; } = String.Empty;
        public string type { get; set; } = String.Empty;
        public string market_name { get; set; } = String.Empty;
        public string market_hash_name { get; set; } = String.Empty;
        public int marketable { get; set; }
    }
}
