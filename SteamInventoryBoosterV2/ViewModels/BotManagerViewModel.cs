using OpenQA.Selenium.BiDi.Modules.Storage;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.SteamApi.Processed;
using SteamInventoryBoosterV2.Models.SteamApi.Response;
using SteamInventoryBoosterV2.Models.Utilities;
using SteamInventoryBoosterV2.Services;
using SteamInventoryBoosterV2.ViewModels.Commands;
using SteamInventoryBoosterV2.ViewModels.ItemViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using static SteamInventoryBoosterV2.ViewModels.ItemViewModels.LogItemModel;

namespace SteamInventoryBoosterV2.ViewModels
{
    public class BotManagerViewModel : BaseViewModel
    {
        private readonly IWebScraper _webScraper;
        private readonly ISteamApiService _steamApiService;
        private readonly ISteamItemService _steamItemService;
        public BotManagerViewModel(IWebScraper webScraper, ISteamApiService steamApiService, ISteamItemService steamItemService)
        {
            _webScraper = webScraper ?? throw new ArgumentNullException(nameof(webScraper));
            _steamApiService = steamApiService ?? throw new ArgumentNullException(nameof(steamApiService));
            _steamItemService = steamItemService ?? throw new ArgumentNullException(nameof(steamItemService));

            // Commands
            StartStopCommand = new RelayCommandAsync(StartOrStop, _ => CanStartOrStop);
            ClearCommand = new RelayCommand(ClearLogs, _ => true);

            LogItems = new ObservableCollection<LogItemModel>();
            _steamItemService = steamItemService;
        }

        // Commands
        public ICommand StartStopCommand { get; }
        public ICommand ClearCommand { get; }


        private ObservableCollection<LogItemModel> _logItems;
        public ObservableCollection<LogItemModel> LogItems
        {
            get => _logItems;
            set
            {
                _logItems = value;
                OnPropertyChanged(nameof(LogItems));
            }
        }

        private TypeOfFunctions? _selectedFunction;
        public TypeOfFunctions? SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                if (_selectedFunction != value)
                {
                    _selectedFunction = value;
                    OnPropertyChanged(nameof(SelectedFunction));
                    OnPropertyChanged(nameof(CanStartOrStop));
                    ((RelayCommandAsync)StartStopCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public enum TypeOfFunctions
        {
            SellItems,
            UpdateItemsInfo,
            UpdateOwnedItemsInfo
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(CanStartOrStop));
                OnPropertyChanged(nameof(IsComboBoxEnabled));
                OnPropertyChanged(nameof(StartStopButtonText));
                OnPropertyChanged(nameof(StartStopButtonColor));
            }
        }

        public bool CanStartOrStop => SelectedFunction.HasValue;
        public bool IsComboBoxEnabled => !IsRunning;
        public string StartStopButtonText => IsRunning ? "Stop" : "Start";
        public Brush StartStopButtonColor => IsRunning ? Brushes.Red : Brushes.Green;

        private CancellationTokenSource? _cancellationTokenSource;

        private async Task StartOrStop(object parameter)
        {
            if (IsRunning)
            {
                _cancellationTokenSource?.Cancel();
                IsRunning = false;
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            IsRunning = true;

            try
            {
                switch (SelectedFunction)
                {
                    case TypeOfFunctions.SellItems:
                        await SellSteamItem(token);
                        break;

                    case TypeOfFunctions.UpdateItemsInfo:
                        await UpdateSteamItemsInformation(token);
                        break;

                    case TypeOfFunctions.UpdateOwnedItemsInfo:
                        await UpdateOwnedItemsInformation(token);
                        break;

                    default:
                        Console.WriteLine($"Unsupported function: {SelectedFunction}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing function {SelectedFunction}: {ex.Message}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void ClearLogs(object? obj)
        {
            LogItems.Clear();
        }

        // In the future, try to make these variables global variables in the application
        private IReadOnlyCollection<OpenQA.Selenium.Cookie>? _steamCookies = null;
        private string? _cookiesString = String.Empty;
        private string? _sessionId = String.Empty;

        private async Task SellSteamItem(CancellationToken token)
        {
            try
            {
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Started, "Starting the functionality of selling steam items."));

                if (!await TryGetSteamCookiesAsync()) return;
                token.ThrowIfCancellationRequested();

                var inventoryInfo = await TryGetInventoryInfoAsync();
                if (inventoryInfo == null) return;
                token.ThrowIfCancellationRequested();

                var tradableItems = TryGetTradableItemsAsync(inventoryInfo);
                if (tradableItems == null) return;

                LogItems.Add(new LogItemModel("Selling Items", String.Empty, LogStatus.Started, "Starting the process of listing Steam items for sale."));

                foreach (var item in tradableItems)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(500, token);

                    await ProcessItemSaleAsync(item);
                }

                LogItems.Add(new LogItemModel("Selling Items", String.Empty, LogStatus.Finished, "Finished listing Steam items for sale."));

                LogFinishSelling();
            }
            catch (OperationCanceledException)
            {
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Stopped, "Item sales function stopped by the user."));
                LogFinishSelling();
                Console.WriteLine("Item sales function stopped by the user.");
            }
            catch (Exception ex)
            {
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Failure, "An internal error occurred. The function will be terminated."));
                LogFinishSelling();
                Console.WriteLine($"Internal in selling steam items function: {ex}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task UpdateSteamItemsInformation(CancellationToken token)
        {
            try
            {
                LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Started, "Starting the functionality of updating Steam items information."));

                var steamItems = await GetSteamItemsFromDatabaseAsync();
                if (steamItems == null) return;
                token.ThrowIfCancellationRequested();

                foreach (var item in steamItems)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(5000, token);

                    await UpdateSteamItemAsync(item);
                }

                LogFinishUpdatingStemItemsInfo();
            }
            catch (OperationCanceledException)
            {
                LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Stopped, "Update steam items information stopped by the user."));
                LogFinishUpdatingStemItemsInfo();
                Console.WriteLine("Update steam items information function stopped by the user.");
            }
            catch (Exception ex)
            {
                LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Failure, "An internal error occurred. The function will be terminated."));
                LogFinishUpdatingStemItemsInfo();
                Console.WriteLine($"Internal in updating Steam items information function: {ex}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task UpdateOwnedItemsInformation(CancellationToken token)
        {
            try
            {
                LogItems.Add(new LogItemModel("Update Owned Items Function", String.Empty, LogStatus.Started, "Starting the functionality of updating owned items information."));

                if (!await TryGetSteamCookiesAsync()) return;

                if (token.IsCancellationRequested) throw new OperationCanceledException();


            }
            catch (OperationCanceledException)
            {
                //LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Stopped, "Update steam items information stopped by the user."));
                //LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Finished, "Finished the functionality of updating Steam items information."));
                //Console.WriteLine("Update steam items information function stopped by the user.");
            }
            catch (Exception ex)
            {
                //LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Failure, "An internal error occurred. The function will be terminated."));
                //LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of updating Steam items information."));
                //Console.WriteLine($"Internal in updating Steam items information function: {ex}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task<bool> TryGetSteamCookiesAsync()
        {
            LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.InProgress, "Getting session cookies."));

            Result<IReadOnlyCollection<OpenQA.Selenium.Cookie>> getCookiesResult = await _webScraper.GetUserSteamCookies();

            if (!getCookiesResult.IsSuccess || getCookiesResult.Value == null)
            {
                LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.Failure, getCookiesResult.Error!.Message));
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                return false;
            }

            _steamCookies = getCookiesResult.Value;
            _cookiesString = String.Join("; ", _steamCookies.Select(c => $"{c.Name}={c.Value}"));
            _sessionId = _steamCookies.FirstOrDefault(cookie => cookie.Name.Equals("sessionid", StringComparison.OrdinalIgnoreCase))?.Value;

            if (_sessionId == null)
            {
                LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.Failure, "Error finding SessionId in cookies."));
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                return false;
            }

            LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.Success, "Session cookies obtained successfully."));
            return true;
        }

        private async Task<GetInventoryInfoResponse?> TryGetInventoryInfoAsync()
        {
            LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.InProgress, "Starting to retrieve inventory information."));

            var getInventoryInfoResponse = await _steamApiService.GetInventoryInfoAsync(_cookiesString);

            if (!getInventoryInfoResponse.IsSuccess || getInventoryInfoResponse.Value == null)
            {
                LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Failure, getInventoryInfoResponse.Error!.Message));
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                return null;
            }

            LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Success,
                $"Inventory information extracted successfully. {getInventoryInfoResponse.Value.total_inventory_count} items were detected in the Inventory."));

            return getInventoryInfoResponse.Value;
        }

        private List<SteamAssetItem>? TryGetTradableItemsAsync(GetInventoryInfoResponse inventoryInfo)
        {
            LogItems.Add(new LogItemModel("Processing Inventory", string.Empty, LogStatus.InProgress, "Processing inventory information to uncover tradable items."));

            var tradableItems = InventoryHelper.GetTradableItems(inventoryInfo);

            if (tradableItems == null || tradableItems.Count == 0)
            {
                LogItems.Add(new LogItemModel("Processing Inventory", string.Empty, LogStatus.Finished, "No tradable items were detected in the inventory."));
                LogFinishSelling();
                return null;
            }

            LogItems.Add(new LogItemModel("Processing Inventory", string.Empty, LogStatus.Finished, $"{tradableItems.Count} tradable items were detected in the inventory"));
            return tradableItems;
        }

        private async Task ProcessItemSaleAsync(SteamAssetItem item)
        {
            LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.Started, "Looking for the prices set for this steam item."));

            var getSteamItemResult = await _steamItemService.GetSteamItemByClassIdAsync(item.Classid);

            if (!getSteamItemResult.IsSuccess || getSteamItemResult.Value == null)
            {
                LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.Failure, "No information on this item could be found in the application database."));
                return;
            }

            var price = getSteamItemResult.Value.SellPrice;
            var received = Calculus.BuyerPaysToSellerReceives(price);

            LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.InProgress, $"Putting the item up for sale at {price}€ (Receive: {received}€)"));

            var sellItemResponse = await _steamApiService.SellSteamItemByApiAsync(_cookiesString, _sessionId, item.Assetid, price);

            if (!sellItemResponse.IsSuccess || sellItemResponse.Value == null)
            {
                LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.Failure, sellItemResponse.Error!.Message));
                return;
            }

            var confirmationRequired = sellItemResponse.Value.requires_confirmation == 1;
            var message = confirmationRequired
                ? "Item successfully submitted for sale, but requires additional confirmation."
                : "Item successfully submitted for sale.";

            LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.Success, message));
        }

        private void LogFinishSelling()
        {
            LogItems.Add(new LogItemModel("Selling Function", string.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
        }

        private async Task UpdateSteamItemAsync(SteamItem item)
        {
            LogItems.Add(new LogItemModel("Update Item", item.Name, LogStatus.InProgress, "Trying to update steam item information."));

            var updateResult = await _steamItemService.UpdateSteamItemInfoAsync(item.ClassId);

            if (!updateResult.IsSuccess || updateResult.Value == null)
            {
                LogItems.Add(new LogItemModel("Update Item", item.Name, LogStatus.Failure, updateResult.Error!.Message));
                return;
            }

            LogItems.Add(new LogItemModel("Update Item", item.Name, LogStatus.Success, "Steam item information updated successfully."));
        }

        private async Task<List<SteamItem>?> GetSteamItemsFromDatabaseAsync()
        {
            LogItems.Add(new LogItemModel("Get DB Info", string.Empty, LogStatus.InProgress, "Getting information from the steam items in database."));

            var result = await _steamItemService.GetAllItemsAsync();

            if (!result.IsSuccess || result.Value == null)
            {
                LogItems.Add(new LogItemModel("Get DB Info", string.Empty, LogStatus.Failure, result.Error!.Message));
                LogItems.Add(new LogItemModel("Update Function", string.Empty, LogStatus.Finished, "Finished the functionality of updating Steam items information."));
                return null;
            }

            LogItems.Add(new LogItemModel("Get DB Info", string.Empty, LogStatus.Success, "DB Steam items information obtained successfully."));
            return result.Value;
        }

        private void LogFinishUpdatingStemItemsInfo()
        {
            LogItems.Add(new LogItemModel("Update Function", String.Empty, LogStatus.Finished, "Finished the functionality of updating Steam items information."));
        }
    }
}
