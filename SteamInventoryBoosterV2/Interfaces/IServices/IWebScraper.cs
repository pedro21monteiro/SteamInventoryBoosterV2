using OpenQA.Selenium;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Utilities;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface IWebScraper
    {
        Task<Result<IReadOnlyCollection<Cookie>>> GetUserSteamCookies();
        Task<Result<SteamItemInfoDto>> CollectBasicSteamItemInformation(string link);
    }
}
