using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SteamInventoryBoosterV2.Services
{
    public class WebScraper : IWebScraper, IDisposable
    {
        private readonly string _urlInventoryPage = "https://steamcommunity.com/profiles/76561198295773303/inventory";
        private readonly string _urlSteamCommunity = "https://steamcommunity.com/market/";

        private readonly string debuggerAddress = "127.0.0.1:9999";

        private readonly ChromeDriver _driverSteamItens;
        private readonly ChromeDriver _driverInventory;

        private readonly SemaphoreSlim _semaphoreSteamItens;
        private readonly SemaphoreSlim _semaphoreInventory;

        public WebScraper()
        {
            _semaphoreSteamItens = new SemaphoreSlim(1, 1);
            _semaphoreInventory = new SemaphoreSlim(1, 1);

            var options = new ChromeOptions();
            options.DebuggerAddress = debuggerAddress;

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driverSteamItens = new ChromeDriver(service, options);
            _driverInventory = new ChromeDriver(service, options);

            _driverInventory.SwitchTo().NewWindow(WindowType.Tab);
        }

        public async Task<Result<IReadOnlyCollection<Cookie>>> GetUserSteamCookies()
        {
            try
            {
                await _driverSteamItens.Navigate().GoToUrlAsync(_urlSteamCommunity);

                IReadOnlyCollection<Cookie>? cookies = _driverSteamItens.Manage().Cookies.AllCookies;

                if (cookies == null || !cookies.Any())
                {
                    return Result<IReadOnlyCollection<Cookie>>.Failure(Error.CookiesNotFound);
                }

                return Result<IReadOnlyCollection<Cookie>>.Success(cookies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to get cookies: {ex.Message}");
                return Result<IReadOnlyCollection<Cookie>>.Failure(Error.InternalError);
            }
        }

        public async Task<Result<SteamItemInfoDto>> CollectBasicSteamItemInformation(string link)
        {
            await _semaphoreSteamItens.WaitAsync();
            try
            {
                _driverSteamItens.Navigate().GoToUrl(link);

                // Create an instance of WebDriverWait to use explicit waits
                WebDriverWait wait = new WebDriverWait(_driverSteamItens, TimeSpan.FromSeconds(5));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

                SteamItemInfoDto steamItemInfoDto = new SteamItemInfoDto();
                steamItemInfoDto.Link = link;

                // Extract Name
                IWebElement elementName = wait.Until(driver =>
                {
                    return driver.FindElement(By.CssSelector(".hover_item_name"));
                });
                steamItemInfoDto.Name = elementName.Text;

                // Extract Actual Price
                IWebElement elementAtualPrice = wait.Until(driver =>
                {
                    return driver.FindElement(By.CssSelector("#market_commodity_forsale > span:nth-child(2)"));
                });
                string atualPriceString = elementAtualPrice.Text.Replace("€", "");
                steamItemInfoDto.Price = double.Parse(atualPriceString);

                // Extract Highest Purchase Order
                IWebElement elementHighestPurchaseOrder = wait.Until(driver =>
                {
                    return driver.FindElement(By.CssSelector("#market_commodity_buyrequests > span:nth-child(2)"));
                });
                string highestPurchaseOrderString = elementHighestPurchaseOrder.Text.Replace("€", "");
                steamItemInfoDto.HighestPurchaseOrder = double.Parse(highestPurchaseOrderString);

                return Result<SteamItemInfoDto>.Success(steamItemInfoDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to get steam item info for {link}: {ex.Message}");
                return Result<SteamItemInfoDto>.Failure(Error.InternalError);
            }
            finally
            {
                _semaphoreSteamItens.Release();
            }
        }

        public void Dispose()
        {
            _driverSteamItens.Quit();
            _driverSteamItens.Dispose();

            _driverInventory.Quit();
            _driverInventory.Dispose();
        }
    }
}
