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

        // URL for the sales endpoint
        private readonly string _sellItemUrl = "https://steamcommunity.com/market/sellitem/";
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

        public async Task<Result<SellItemResponse>> SellSteamItemByApiAsync(string cookiesString, string sessionId, string assetId, decimal sellPrice)
        {
            // Necessary headers
            string referer = "https://steamcommunity.com/profiles/76561198295773303/inventory";

            var sellerReceives = Calculus.BuyerPaysToSellerReceives(sellPrice);
            string priceIntString = Calculus.ConvertDecimalToInteger(sellerReceives).ToString();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add the headers
                    httpClient.DefaultRequestHeaders.Add("cookie", cookiesString);
                    httpClient.DefaultRequestHeaders.Add("referer", referer);
                    httpClient.DefaultRequestHeaders.Add("accept-language", "pt-PT,pt;q=0.9,en-US;q=0.8,en;q=0.7");

                    // Setting the parameters in the body (x-www-form-urlencoded)
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("sessionid", sessionId),
                        new KeyValuePair<string, string>("appid", "730"),
                        new KeyValuePair<string, string>("contextid", "2"),
                        new KeyValuePair<string, string>("assetid", assetId),
                        new KeyValuePair<string, string>("amount", "1"),
                        new KeyValuePair<string, string>("price", priceIntString)
                    });

                    // Send the POST request
                    var response = await httpClient.PostAsync(_sellItemUrl, content);

                    var statusCode = response.StatusCode;

                    if (statusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Error sending POST request to sell item. HTTP code: {statusCode}");
                        return Result<SellItemResponse>.Failure(Error.InventoryRetrievalFailed);
                    }

                    // Captures the content of the response
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var sellItemResponse = JObject.Parse(responseContent).ToObject<SellItemResponse>();

                    if (sellItemResponse == null)
                    {
                        Console.WriteLine("Failed to parse inventory response.");
                        return Result<SellItemResponse>.Failure(Error.InternalError);
                    }

                    return Result<SellItemResponse>.Success(sellItemResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to sell steam item: {ex}");
                return Result<SellItemResponse>.Failure(Error.InternalError);
            }
        }

    }
}
