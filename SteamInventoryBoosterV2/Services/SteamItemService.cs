using SteamInventoryBoosterV2.Interfaces.IDao;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Services
{
    public class SteamItemService : ISteamItemService
    {
        private readonly ISteamItemDao _steamItemDao;
        public SteamItemService(ISteamItemDao steamItemDao)
        {
            _steamItemDao = steamItemDao ?? throw new ArgumentNullException(nameof(steamItemDao));
        }

        public async Task<Result<SteamItem>> CreateSteamItemAsync(string link)
        {
            var itemExists = await _steamItemDao.ItemExistsByLinkAsync(link);
            if (itemExists)
            {
                return Result<SteamItem>.Failure(Error.SteamItemAlreadyExists);
            }

            return Result<SteamItem>.Success(new SteamItem());


            // Vai ter de coletar os dados do item

            // Vai ter de fazer download da imagem e guardar numa pasta




            //// Se não existe, criar o novo item
            //try
            //{
            //    var createdItem = await _steamItemDao.CreateSteamItemAsync(steamItem);
            //    if (createdItem != null)
            //    {
            //        return Result<SteamItem>.Success(createdItem); // Criação bem-sucedida
            //    }
            //    else
            //    {
            //        return Result<SteamItem>.Failure(null); // Falha ao criar o item
            //    }
            //}
            //catch (Exception)
            //{
            //    return Result<SteamItem>.Failure(null); // Falha no processo de criação
            //}
        }

    }
}
