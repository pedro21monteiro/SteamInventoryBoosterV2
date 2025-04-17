using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Domain
{
    public class OwnedItem
    {
        [Key]
        public int Id { get; set; }
        public string ClassId { get; set; } = String.Empty;
        public string AssetId { get; set; } = String.Empty;
        public DateTime AcquiredAt { get; set; }
        public DateTime? SoldAt { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? ListedPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public ItemStatus Status { get; set; }
        public SteamItem? SteamItem { get; set; }

        public enum ItemStatus
        {
            InInventory = 0,
            ListedForSale = 1,
            Sold = 2
        }
    }
}
