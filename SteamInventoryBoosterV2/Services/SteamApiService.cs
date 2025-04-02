using Newtonsoft.Json.Linq;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.SteamApi.Response;
using SteamInventoryBoosterV2.Models.Utilities;
using System.Net;
using System.Net.Http;

namespace SteamInventoryBoosterV2.Services
{
    public class SteamApiService : ISteamApiService
    {
        // Can only place orders for 5000 items of inventory, if it go over I'll have to find another solution
        private readonly string _urlInventoryInfo = "https://steamcommunity.com/inventory/76561198295773303/730/2?l=portuguese&count=5000";

        public async Task<Result<GetInventoryInfoResponse>> GetInventoryInfoAsync(string cookiesString)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add the cookie header
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookiesString);

                    // Performs GET on the URL
                    var response = await httpClient.GetAsync(_urlInventoryInfo);

                    HttpStatusCode statusCode = response.StatusCode;

                    if (statusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Error retrieving inventory information. HTTP code: {statusCode}");
                        return Result<GetInventoryInfoResponse>.Failure(Error.InventoryRetrievalFailed);
                    }


                    var jsonString = await response.Content.ReadAsStringAsync();

                    var inventoryResponse = JObject.Parse(jsonString).ToObject<GetInventoryInfoResponse>();

                    if (inventoryResponse == null)
                    {
                        Console.WriteLine("Failed to parse inventory response.");
                        return Result<GetInventoryInfoResponse>.Failure(Error.InternalError);
                    }

                    return Result<GetInventoryInfoResponse>.Success(inventoryResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to obtain inventory information: {ex}");
                return Result<GetInventoryInfoResponse>.Failure(Error.InternalError);
            }
        }
    }
}
