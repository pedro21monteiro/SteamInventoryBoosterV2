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
        public double Price { get; set; }
        public double HighestPurchaseOrder { get; set; }
        public DateTime LastVerification { get; set; }
        public double? BuyPrice { get; set; }
        public double? SellPrice { get; set; }
        public string IconPath { get; set; } = string.Empty;
    }
}
