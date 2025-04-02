using SteamInventoryBoosterV2.Models.SteamApiResponse;
using SteamInventoryBoosterV2.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface ISteamApiService
    {
        Task<Result<GetInventoryInfoResponse>> GetInventoryInfoAsync(string cookiesString);
    }
}
