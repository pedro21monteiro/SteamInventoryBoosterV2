using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface IWebScraper
    {
        Task<Result<IReadOnlyCollection<Cookie>>> GetUserSteamCookies();
        Task<Result<SteamItemInfoDto>> CollectBasicSteamItemInformation(string link);
    }
}
