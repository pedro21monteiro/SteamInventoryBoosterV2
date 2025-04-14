using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SteamInventoryBoosterV2.Models.Domain
{
    public class SteamItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ClassId { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal HighestPurchaseOrder { get; set; }
        public DateTime LastVerification { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
    }
}
