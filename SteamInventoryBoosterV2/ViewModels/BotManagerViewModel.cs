using SteamInventoryBoosterV2.Interfaces.IServices;
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
            UpdatePrices
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

            if (SelectedFunction == TypeOfFunctions.SellItems)
            {
                await SellSteamItem(token);
            }
            else if (SelectedFunction == TypeOfFunctions.UpdatePrices)
            {
                await UpdateSteamItemsPrices(token);
            }
            else
            {
                IsRunning = false;
            }
            IsRunning = false;
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

                LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.InProgress, "Getting session cookies."));

                Result<IReadOnlyCollection<OpenQA.Selenium.Cookie>> getCookiesResult = await _webScraper.GetUserSteamCookies();

                if (!getCookiesResult.IsSuccess || getCookiesResult.Value == null)
                {
                    LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.Failure, getCookiesResult.Error!.Message));
                    LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                    return;
                }

                _steamCookies = getCookiesResult.Value;
                LogItems.Add(new LogItemModel("Get Cookies", String.Empty, LogStatus.Success, "Session cookies obtained successfully."));

                if (token.IsCancellationRequested) throw new OperationCanceledException();

                LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.InProgress, "Starting to retrieve inventory information."));

                _cookiesString = String.Join("; ", _steamCookies.Select(c => $"{c.Name}={c.Value}"));
                _sessionId = _steamCookies.FirstOrDefault(cookie => cookie.Name.Equals("sessionid", StringComparison.OrdinalIgnoreCase))?.Value;

                var getInventoryInfoResponse = await _steamApiService.GetInventoryInfoAsync(_cookiesString);

                if (!getInventoryInfoResponse.IsSuccess || getInventoryInfoResponse.Value == null)
                {
                    LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Failure, getInventoryInfoResponse.Error!.Message));
                    LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                    return;
                }

                LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Success,
                    $"Inventory information extracted successfully. {getInventoryInfoResponse.Value.total_inventory_count} items were detected in the Inventory."));

                if (token.IsCancellationRequested) throw new OperationCanceledException();

                LogItems.Add(new LogItemModel("Processing Inventory", String.Empty, LogStatus.InProgress, "Processing inventory information to uncover tradable items."));

                var tradableItens = InventoryHelper.GetTradableItems(getInventoryInfoResponse.Value);

                if(tradableItens == null || tradableItens.Count == 0)
                {
                    LogItems.Add(new LogItemModel("Processing Inventory", String.Empty, LogStatus.Finished, getInventoryInfoResponse.Error!.Message));
                    LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "No tradable items were detected in the inventory."));
                    return;
                }

                LogItems.Add(new LogItemModel("Processing Inventory", String.Empty, LogStatus.Finished, $"{tradableItens.Count} tradable items were detected in the inventory"));

                LogItems.Add(new LogItemModel("Selling Items", String.Empty, LogStatus.Started, "Starting the process of listing Steam items for sale."));

                foreach(var item in tradableItens)
                {
                    // Put up for sale every 500 milliseconds
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested) throw new OperationCanceledException();

                    LogItems.Add(new LogItemModel("Selling Item", item.MarketName, LogStatus.Started, "Looking for the prices set for this steam item."));

                    var getSteamItemResult = await _steamItemService.GetSteamItemByNameAsync(item.MarketName);

                    if(!getSteamItemResult.IsSuccess || getSteamItemResult.Value == null)
                    {
                        LogItems.Add(new LogItemModel("Selling Items", item.MarketName, LogStatus.Failure, "No information on this item could be found in the application database."));
                        continue;
                    }

                    // vai vender o item e esperar a resposta

                }








                await Task.Delay(10000, token);
                if (token.IsCancellationRequested) return;

                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
            }
            catch (OperationCanceledException)
            {
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Stopped, "Item sales function stopped by the user."));
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                Console.WriteLine("Item sales function stopped by the user.");
            }
            catch (Exception ex)
            {
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Failure, "An internal error occurred. The function will be terminated."));
                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                Console.WriteLine($"Internal in selling steam items function: {ex}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task UpdateSteamItemsPrices(CancellationToken token)
        {
            //try
            //{

            //    await Task.Delay(10000, token);
            //    if (token.IsCancellationRequested) return;
            //}
            //catch (OperationCanceledException)
            //{
            //    Console.WriteLine("Item sales task canceled by the user.");
            //}
            //finally
            //{
            //    IsRunning = false;
            //}
        }
    }
}
