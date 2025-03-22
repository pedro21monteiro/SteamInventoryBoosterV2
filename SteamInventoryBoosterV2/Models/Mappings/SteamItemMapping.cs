using OpenQA.Selenium;
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
                Price = model.Price,
                HighestPurchaseOrder = model.HighestPurchaseOrder,
                LastVerification = model.LastVerification,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                IconPath = model.IconPath
            };
        }

        public static SteamItemModel ToItemViewModel(SteamItem entity)
        {
            return new SteamItemModel
            {
                Id = entity.Id,
                Link = entity.Link,
                Name = entity.Name,
                BuyPrice = entity.BuyPrice,
                SellPrice = entity.SellPrice,
                Price = entity.Price,
                HighestPurchaseOrder = entity.HighestPurchaseOrder,
                LastVerification = entity.LastVerification,
                IconPath = entity.IconPath
            };
        }

        public static void UpdateEntityInfo(SteamItem existingItem, SteamItemInfoDto dto)
        {
            existingItem.Name = dto.Name;
            existingItem.Price = dto.Price;
            existingItem.HighestPurchaseOrder = dto.HighestPurchaseOrder;
            existingItem.LastVerification = dto.LastVerification;
        }

        public static void UpdateItemViewModelInfo(SteamItemModel model, SteamItem entity)
        {
            model.Name = entity.Name;
            model.Price = entity.Price;
            model.HighestPurchaseOrder = entity.HighestPurchaseOrder;
            model.LastVerification = entity.LastVerification;
        }
    }
}
