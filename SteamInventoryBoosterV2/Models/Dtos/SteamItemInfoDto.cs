namespace SteamInventoryBoosterV2.Models.Dtos
{
    public class SteamItemInfoDto
    {
        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal HighestPurchaseOrder { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public DateTime LastVerification { get; set; }
    }
}
