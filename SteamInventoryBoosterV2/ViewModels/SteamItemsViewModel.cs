using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Mappings;
using SteamInventoryBoosterV2.Models.Utilities;
using SteamInventoryBoosterV2.Services;
using SteamInventoryBoosterV2.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class SteamItemsViewModel : BaseViewModel
    {
        private readonly string _chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        public readonly ISteamItemService _steamItemService;
        public readonly IMessageBoxService _messageBoxService;
        public SteamItemsViewModel(ISteamItemService steamItemService, IMessageBoxService messageBoxService)
        {
            _steamItemService = steamItemService ?? throw new ArgumentNullException(nameof(steamItemService));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));

            AddItemCommand = new RelayCommandAsync(CreateStemItemAsync, CanAddSteamItem);
            GoToPageCommand = new RelayCommand(GoToStemItemPage, _ => true);
            EditItemCommand = new RelayCommandAsync(EditSteamItemAsync, IsBuyAndSellPricesDefined);
            RemoveItemCommand = new RelayCommandAsync(RemoveSteamItemAsync, _ => true);

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>();

            LoadSteamItems();
        }

        // Commands
        public ICommand AddItemCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public SteamItemModel SelectedItem { get; set; }

        private ObservableCollection<SteamItemModel> _filteredSteamItensInformation;
        public ObservableCollection<SteamItemModel> FilteredSteamItensInformation
        {
            get => _filteredSteamItensInformation;
            set
            {
                _filteredSteamItensInformation = value;
                OnPropertyChanged(nameof(FilteredSteamItensInformation));
            }
        }

        private string _linkInput;
        public string LinkInput
        {
            get => _linkInput;
            set
            {
                if (SetProperty(ref _linkInput, value))
                {
                    ((RelayCommandAsync)AddItemCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _buyPriceInput;
        public string BuyPriceInput
        {
            get => _buyPriceInput;
            set
            {
                if (SetProperty(ref _buyPriceInput, value))
                {
                    CalculateProfit();
                    ((RelayCommandAsync)AddItemCommand).RaiseCanExecuteChanged();
                    ((RelayCommandAsync)EditItemCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _sellPriceInput;
        public string SellPriceInput
        {
            get => _sellPriceInput;
            set
            {
                if (SetProperty(ref _sellPriceInput, value))
                {
                    CalculateProfit();
                    ((RelayCommandAsync)AddItemCommand).RaiseCanExecuteChanged();
                    ((RelayCommandAsync)EditItemCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _profitInput;
        public string ProfitInput
        {
            get => _profitInput;
            private set => SetProperty(ref _profitInput, value);
        }

        private string _profitPercentageInput;
        public string ProfitPercentageInput
        {
            get => _profitPercentageInput;
            private set => SetProperty(ref _profitPercentageInput, value);
        }

        private bool CanAddSteamItem(object parameter)
        {
            return IsBuyAndSellPricesDefined(parameter)
                && !string.IsNullOrEmpty(LinkInput);
        }

        private bool IsBuyAndSellPricesDefined(object parameter)
        {
            return !string.IsNullOrEmpty(BuyPriceInput)
                && !string.IsNullOrEmpty(SellPriceInput)
                && ProfitInput != "N/A"
                && ProfitPercentageInput != "N/A";
        }

        private void CalculateProfit()
        {
            if (decimal.TryParse(BuyPriceInput, out var buy) && decimal.TryParse(SellPriceInput, out var sell) && IsValidDecimal(BuyPriceInput) && IsValidDecimal(SellPriceInput))
            {
                ProfitInput = Calculus.SellItemProfit(buy, sell).ToString("F2");

                if (buy != 0)
                {
                    ProfitPercentageInput = Calculus.SellItemProfitPercentage(buy, sell).ToString("F2") + "%";
                }
                else
                {
                    ProfitPercentageInput = "N/A";
                }
            }
            else
            {
                ProfitInput = "N/A";
                ProfitPercentageInput = "N/A";
            }
        }

        // Verify that it has a maximum of 2 decimal places
        private bool IsValidDecimal(string input)
        {
            if (decimal.TryParse(input, out var value))
            {
                var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
                return decimalPlaces <= 2;
            }
            return false;
        }

        private async Task CreateStemItemAsync(object parameter)
        {
            if (!decimal.TryParse(BuyPriceInput, out decimal buyPrice) || !decimal.TryParse(SellPriceInput, out decimal sellPrice))
            {
                _messageBoxService.ShowWarning("Please enter valid prices.");
                return;
            }

            Result<SteamItem> createSteamItemResult = await _steamItemService.CreateSteamItemAsync(LinkInput, buyPrice, sellPrice);

            if (!createSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to create the item: {createSteamItemResult.Error!.Message}");
                return;                
            }

            _messageBoxService.ShowSuccess("Item created successfully!");

            SteamItemModel steamItemModel = SteamItemMapping.ToItemViewModel(createSteamItemResult.Value!);
            FilteredSteamItensInformation.Add(steamItemModel);
        }

        public async void LoadSteamItems()
        {
            var getItemsResult = await _steamItemService.GetAllItemsAsync();
            FilteredSteamItensInformation.Clear();

            if (!getItemsResult.IsSuccess || getItemsResult.Value == null)
            {
                return;
            }

            foreach (var item in getItemsResult.Value)
            {
                FilteredSteamItensInformation.Add(SteamItemMapping.ToItemViewModel(item));
            }
        }

        private void GoToStemItemPage(object? arg)
        {
            if (string.IsNullOrWhiteSpace(SelectedItem.Link))
            {
                _messageBoxService.ShowError("The selected item does not have a valid link.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _chromePath,
                    Arguments = SelectedItem.Link,
                    UseShellExecute = false
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal error while trying to open the link {SelectedItem.Link}: {ex.Message} \n{ex}");
                _messageBoxService.ShowError($"Error opening link: {ex.Message}");
            }
        }

        private async Task EditSteamItemAsync(object parameter)
        {
            if (!decimal.TryParse(BuyPriceInput, out decimal buyPrice) || !decimal.TryParse(SellPriceInput, out decimal sellPrice))
            {
                _messageBoxService.ShowWarning("Please enter valid prices.");
                return;
            }

            var confirmResult = _messageBoxService.ShowYesNo($"Are you sure you want to update the Buy Price to {buyPrice} and Sell Price to {sellPrice}?");
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            Result<SteamItem> editSteamItemResult = await _steamItemService.EditItemBuySellPricesAsync(SelectedItem.Id, buyPrice, sellPrice);

            if (!editSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to edit the item: {editSteamItemResult.Error!.Message}");
                return;
            }

            SelectedItem.BuyPrice = buyPrice;
            SelectedItem.SellPrice = sellPrice;

            _messageBoxService.ShowSuccess("Item edited successfully!");
        }

        private async Task RemoveSteamItemAsync(object parameter)
        {
            var confirmResult = _messageBoxService.ShowYesNo($"Are you sure you want to delete the steam item {SelectedItem.Name}?");
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            Result deleteSteamItemResult = await _steamItemService.DeleteSteamItemAsync(SelectedItem.Id);

            if (!deleteSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to delete the item: {deleteSteamItemResult.Error!.Message}");
                return;
            }

            FilteredSteamItensInformation.Remove(SelectedItem);

            _messageBoxService.ShowSuccess("Item deleted successfully!");
        }
    }
}
