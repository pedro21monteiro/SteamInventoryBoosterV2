using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Domain
{
    public class SteamItem
    {
        [Key]
        public int Id { get; set; }
        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal HighestPurchaseOrder { get; set; }
        public DateTime LastVerification { get; set; }
        public decimal? BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public string IconPath { get; set; } = string.Empty;
    }
}
