using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.SteamApi.Response
{
    public class SellItemResponse
    {
        public bool success { get; set; }
        public int? requires_confirmation { get; set; }
        public string? message { get; set; }
    }
}
