using SteamInventoryBoosterV2.Models.SteamApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.SteamApiResponse
{
    public class GetInventoryInfoResponse
    {
        public List<Asset> assets { get; set; } = new List<Asset>();
        public List<Description> descriptions { get; set; } = new List<Description>();
        public int total_inventory_count { get; set; }
        public int success { get; set; }
    }
}
