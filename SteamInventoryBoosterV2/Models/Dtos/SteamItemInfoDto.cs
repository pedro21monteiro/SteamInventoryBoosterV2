namespace SteamInventoryBoosterV2.Models.Dtos
{
    public class SteamItemInfoDto
    {
        public string ClassId { get; set; } = String.Empty;
        public string Link { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public decimal HighestPurchaseOrder { get; set; }
        public string IconUrl { get; set; } = String.Empty;
        public DateTime LastVerification { get; set; }
    }
}
