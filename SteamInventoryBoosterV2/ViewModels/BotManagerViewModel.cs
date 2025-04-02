using Microsoft.IdentityModel.Tokens;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Utilities;
using SteamInventoryBoosterV2.ViewModels.Commands;
using SteamInventoryBoosterV2.ViewModels.ItemViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static SteamInventoryBoosterV2.ViewModels.ItemViewModels.LogItemModel;
using static SteamInventoryBoosterV2.ViewModels.ItemViewModels.SteamItemsViewModel;

namespace SteamInventoryBoosterV2.ViewModels
{
    public class BotManagerViewModel : BaseViewModel
    {
        private readonly IWebScraper _webScraper;
        private readonly ISteamApiService _steamApiService;
        public BotManagerViewModel(IWebScraper webScraper, ISteamApiService steamApiService)
        {
            _webScraper = webScraper ?? throw new ArgumentNullException(nameof(webScraper));
            _steamApiService = steamApiService ?? throw new ArgumentNullException(nameof(steamApiService));

            // Commands
            StartStopCommand = new RelayCommandAsync(StartOrStop, _ => CanStartOrStop);

            LogItems = new ObservableCollection<LogItemModel>();
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

                if (token.IsCancellationRequested) return;

                LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.InProgress, "Starting to retrieve inventory information."));

                _cookiesString = String.Join("; ", _steamCookies.Select(c => $"{c.Name}={c.Value}"));
                _sessionId = _steamCookies.FirstOrDefault(cookie => cookie.Name.Equals("sessionid", StringComparison.OrdinalIgnoreCase))?.Value;

                var getInventoryInfoResponse = await _steamApiService.GetInventoryInfoAsync(_cookiesString);

                if(!getInventoryInfoResponse.IsSuccess || getInventoryInfoResponse.Value == null)
                {
                    LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Failure, getInventoryInfoResponse.Error!.Message));
                    LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
                    return;
                }

                LogItems.Add(new LogItemModel("Get Inventory Info", String.Empty, LogStatus.Success, 
                    $"Inventory information extracted successfully. {getInventoryInfoResponse.Value.total_inventory_count} items were detected in the Inventory."));







                await Task.Delay(10000, token);
                if (token.IsCancellationRequested) return;

                LogItems.Add(new LogItemModel("Selling Function", String.Empty, LogStatus.Finished, "Finished the functionality of selling Steam items."));
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Item sales task canceled by the user.");
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
