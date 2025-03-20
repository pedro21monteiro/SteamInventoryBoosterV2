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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class SteamItemsViewModel : BaseViewModel
    {
        public readonly ISteamItemService _steamItemService;
        public readonly IMessageBoxService _messageBoxService;
        public SteamItemsViewModel(ISteamItemService steamItemService, IMessageBoxService messageBoxService)
        {
            _steamItemService = steamItemService ?? throw new ArgumentNullException(nameof(steamItemService));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));

            AddItemCommand = new RelayCommandAsync(CreateStemItemAsync, IsBuyAndSellPricesDefined);
            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>();
            //FilteredSteamItensInformation.Add(new SteamItemModel { Name = "Item 1", BuyPrice = 2.5, SellPrice = 3.0, LastVerification = DateTime.Now, AtualPrice = 3.2, HighestPurchaseOrder = 2.8, IconPath = "/Assets/AppImage.png" });
        }

        // Commands
        public ICommand AddItemCommand { get; }

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

        private bool IsBuyAndSellPricesDefined(object parameter)
        {
            return !string.IsNullOrEmpty(BuyPriceInput)
                && !string.IsNullOrEmpty(SellPriceInput)
                && !string.IsNullOrEmpty(LinkInput)
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
                MessageBox.Show("Please enter valid prices.");
                return;
            }

            Result<SteamItem> createSteamItemResult = await _steamItemService.CreateSteamItemAsync(LinkInput, buyPrice, sellPrice);

            if (!createSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to create the item: {createSteamItemResult.Error!.Message}");
                return;                
            }

            _messageBoxService.ShowSuccess("Item created successfully!");

            SteamItemModel steamItemModel = SteamItemMapping.ToModel(createSteamItemResult.Value!);
            FilteredSteamItensInformation.Add(steamItemModel);
        }

    }
}
