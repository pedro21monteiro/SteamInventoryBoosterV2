using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Dtos
{
    public class SteamItemInfoDto
    {
        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double HighestPurchaseOrder { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }
}
