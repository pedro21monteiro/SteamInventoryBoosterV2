using SteamInventoryBoosterV2.Models.SteamApi.Response;
using SteamInventoryBoosterV2.Models.Utilities;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface ISteamApiService
    {
        Task<Result<GetInventoryInfoResponse>> GetInventoryInfoAsync(string cookiesString);
    }
}
