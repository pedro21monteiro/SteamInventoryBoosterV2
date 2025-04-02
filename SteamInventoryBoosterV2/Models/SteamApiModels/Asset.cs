using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.SteamApiModels
{
    public class Asset
    {
        public int appid { get; set; }
        public string contextid { get; set; } = String.Empty;
        public string assetid { get; set; } = String.Empty;
        public string classid { get; set; } = String.Empty;
        public string instanceid { get; set; } = String.Empty;
        public string amount { get; set; } = String.Empty;
    }
}
