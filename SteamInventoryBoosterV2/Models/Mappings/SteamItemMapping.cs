using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Services;
using SteamInventoryBoosterV2.ViewModels.ItemViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Mappings
{
    public static class SteamItemMapping
    {
        public static SteamItem ToEntity(SteamItemInfoDto dto)
        {
            return new SteamItem
            {
                Link = dto.Link,
                Name = dto.Name,
                Price = dto.Price,
                HighestPurchaseOrder = dto.HighestPurchaseOrder,
                IconPath = dto.IconUrl,
                LastVerification = dto.LastVerification
            };
        }

        public static SteamItemInfoDto ToDto(SteamItem entity)
        {
            return new SteamItemInfoDto
            {
                Link = entity.Link,
                Name = entity.Name,
                Price = entity.Price,
                HighestPurchaseOrder = entity.HighestPurchaseOrder,
                IconUrl = entity.IconPath,
                LastVerification = entity.LastVerification
            };
        }

        public static SteamItem ToEntity(SteamItemModel model)
        {
            return new SteamItem
            {
                Id = model.Id,
                Link = model.Link,
                Name = model.Name,
                Price = model.AtualPrice,
                HighestPurchaseOrder = model.HighestPurchaseOrder,
                LastVerification = model.LastVerification,
                BuyPrice = model.BuyPrice != 0 ? model.BuyPrice : (double?)null,
                SellPrice = model.SellPrice != 0 ? model.SellPrice : (double?)null,
                IconPath = model.IconPath
            };
        }

        public static SteamItemModel ToModel(SteamItem entity)
        {
            return new SteamItemModel
            {
                Id = entity.Id,
                Link = entity.Link,
                Name = entity.Name,
                BuyPrice = entity.BuyPrice ?? 0,
                SellPrice = entity.SellPrice ?? 0,
                AtualPrice = entity.Price,
                HighestPurchaseOrder = entity.HighestPurchaseOrder,
                LastVerification = entity.LastVerification,
                IconPath = entity.IconPath
            };
        }
    }
}
