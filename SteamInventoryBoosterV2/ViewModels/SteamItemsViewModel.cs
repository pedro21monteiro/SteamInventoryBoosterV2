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
            UpdateItemCommand = new RelayCommandAsync(UpdateSteamItemAsync, _ => true);
            SortCommand = new RelayCommand(SortItems);
            SearchByNameCommand = new RelayCommand(FilterSteamItemsByName, _ => true);
            RefreshPageCommand = new RelayCommand(RefreshSteamItemsInformation, _ => true);
            ChangeVisibilityCommand = new RelayCommandAsync(ChangeVisibilityAsync, _ => true);

            _allSteamItensInformation = new ObservableCollection<SteamItemModel>();
            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>();

            ShowTypeOfItems = TypeOfItems.VisibleItems;
            LoadSteamItems();
        }

        // Commands
        public ICommand AddItemCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand UpdateItemCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand SearchByNameCommand { get; }
        public ICommand RefreshPageCommand { get; }
        public ICommand ChangeVisibilityCommand { get; }

        public SteamItemModel SelectedItem { get; set; }

        private ObservableCollection<SteamItemModel> _allSteamItensInformation { get; set; }

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

        private string _searchTermInput;
        public string SearchTermInput
        {
            get => _searchTermInput;
            set
            {
                _searchTermInput = value;
                OnPropertyChanged(nameof(SearchTermInput));
            }
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

            Result<SteamItem> createSteamItemResult = await _steamItemService.CreateSteamItemAsync(LinkInput.Trim(), buyPrice, sellPrice);

            if (!createSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to create the item: {createSteamItemResult.Error!.Message}");
                return;                
            }

            SteamItemModel steamItemModel = SteamItemMapping.ToItemViewModel(createSteamItemResult.Value!);
            FilteredSteamItensInformation.Add(steamItemModel);
            _allSteamItensInformation.Add(steamItemModel);

            ClearAddItemInputParameters();
            FilterItemsByType();

            _messageBoxService.ShowSuccess("Item created successfully!");
        }

        public async void LoadSteamItems()
        {
            FilteredSteamItensInformation.Clear();
            _allSteamItensInformation.Clear();

            var getItemsResult = await _steamItemService.GetAllItemsAsync();

            if (!getItemsResult.IsSuccess || getItemsResult.Value == null)
            {
                return;
            }

            foreach (var item in getItemsResult.Value)
            {
                _allSteamItensInformation.Add(SteamItemMapping.ToItemViewModel(item));
            }

            FilteredSteamItensInformation = _allSteamItensInformation;
            FilterItemsByType();
        }

        private void GoToStemItemPage(object arg)
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
            SteamItemModel currentItem = SelectedItem;
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

            Result<SteamItem> editSteamItemResult = await _steamItemService.EditItemBuySellPricesAsync(currentItem.Id, buyPrice, sellPrice);

            if (!editSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to edit the item: {editSteamItemResult.Error!.Message}");
                return;
            }

            currentItem.BuyPrice = buyPrice;
            currentItem.SellPrice = sellPrice;

            UpdateItemInAllSteamItensInformationList(currentItem);

            _messageBoxService.ShowSuccess("Item edited successfully!");
        }

        private async Task RemoveSteamItemAsync(object parameter)
        {
            SteamItemModel currentItem = SelectedItem;
            var confirmResult = _messageBoxService.ShowYesNo($"Are you sure you want to delete the steam item {SelectedItem.Name}?");
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            Result deleteSteamItemResult = await _steamItemService.DeleteSteamItemAsync(currentItem.Id);

            if (!deleteSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to delete the item: {deleteSteamItemResult.Error!.Message}");
                return;
            }

            FilteredSteamItensInformation.Remove(currentItem);
            _allSteamItensInformation.Remove(currentItem);

            _messageBoxService.ShowSuccess("Item deleted successfully!");
        }

        private async Task UpdateSteamItemAsync(object parameter)
        {
            SteamItemModel currentItem = SelectedItem;
            Result<SteamItem> updateSteamItemResult = await _steamItemService.UpdateSteamItemInfoAsync(currentItem.Id);

            if (!updateSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to update the item info: {updateSteamItemResult.Error!.Message}");
                return;
            }

            SteamItemMapping.UpdateItemViewModelInfo(currentItem, updateSteamItemResult.Value!);

            UpdateItemInAllSteamItensInformationList(currentItem);

            _messageBoxService.ShowSuccess("Item updated successfully!");
        }

        private async Task ChangeVisibilityAsync(object parameter)
        {
            SteamItemModel currentItem = SelectedItem;

            Result<SteamItem> hideSteamItemResult = await _steamItemService.ChangeVisibilityAsync(currentItem.Id);

            if (!hideSteamItemResult.IsSuccess)
            {
                _messageBoxService.ShowError($"Failed to hide the steam item info: {hideSteamItemResult.Error!.Message}");
                return;
            }

            SteamItemMapping.UpdateItemViewModelInfo(currentItem, hideSteamItemResult.Value!);

            UpdateItemInAllSteamItensInformationList(currentItem);
            FilterItemsByType();
        }

        private string _currentSortProperty = String.Empty;
        private bool _isAscending = true;
        private void SortItems(object parameter)
        {
            var parameterString = parameter.ToString();

            if (parameterString == null)
            {
                Console.WriteLine(Error.SortColumnNotDefined.Message);
                _messageBoxService.ShowError(Error.InternalError.Message);
                return;
            }

            if (_currentSortProperty == parameterString)
            {
                _isAscending = !_isAscending;
            }
            else
            {
                _currentSortProperty = parameterString;
                _isAscending = true;
            }

            switch (_currentSortProperty)
            {
                case "ColumnName":
                    SortByName();
                    break;
                case "ColumnBuyPrice":
                    SortByBuyPrice();
                    break;
                case "ColumnSellPrice":
                    SortBySellPrice();
                    break;
                case "ColumnSellProfit":
                    SortBySellProfit();
                    break;
                case "ColumnProfitPercentage":
                    SortByProfitPercentage();
                    break;
                case "ColumnLastVerification":
                    SortByLastVerification();
                    break;
                case "ColumnPrice":
                    SortByPrice();
                    break;
                case "ColumnHighestPurchaseOrder":
                    SortByHighestPurchaseOrder();
                    break;
                default:
                    Console.WriteLine(Error.SortColumnNotDefined.Message);
                    _messageBoxService.ShowError(Error.SortColumnNotDefined.Message);
                    break;
            }
        }

        private void SortByName()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.Name).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.Name).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }
        private void SortByBuyPrice()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.BuyPrice).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.BuyPrice).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void SortBySellPrice()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.SellPrice).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.SellPrice).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }
        private void SortBySellProfit()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.SellProfit).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.SellProfit).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void SortByProfitPercentage()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.ProfitPercentage).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.ProfitPercentage).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void SortByLastVerification()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.LastVerification).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.LastVerification).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void SortByPrice()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.Price).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.Price).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void SortByHighestPurchaseOrder()
        {
            var sortedList = _isAscending
                ? FilteredSteamItensInformation.OrderBy(item => item.HighestPurchaseOrder).ToList()
                : FilteredSteamItensInformation.OrderByDescending(item => item.HighestPurchaseOrder).ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(sortedList);
        }

        private void FilterSteamItemsByName(object obj)
        {
            var isVisibleCondition = ShowTypeOfItems == TypeOfItems.VisibleItems ? true :
                             ShowTypeOfItems == TypeOfItems.HiddenItems ? false : (bool?)null;

            var filteredItems = _allSteamItensInformation
                .Where(i => (!isVisibleCondition.HasValue || i.IsVisible == isVisibleCondition) &&
                            (string.IsNullOrEmpty(SearchTermInput) || i.Name.Contains(SearchTermInput, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(filteredItems);
        }

        private void UpdateItemInAllSteamItensInformationList(SteamItemModel updatedItem)
        {
            var originalItem = _allSteamItensInformation.FirstOrDefault(i => i.Id == updatedItem.Id);

            originalItem = updatedItem;
        }

        private void ClearAddItemInputParameters()
        {
            LinkInput = "";
            BuyPriceInput = "";
            SellPriceInput = "";
        }

        private void RefreshSteamItemsInformation(object parameter)
        {
            LoadSteamItems();
            _messageBoxService.ShowSuccess("Page successfully refreshed!");
        }

        private TypeOfItems _showTypeOfItems;
        public TypeOfItems ShowTypeOfItems
        {
            get => _showTypeOfItems;
            set
            {
                if (_showTypeOfItems != value)
                {
                    _showTypeOfItems = value;
                    FilterItemsByType();
                    OnPropertyChanged(nameof(ShowTypeOfItems));
                }
            }
        }

        private void FilterItemsByType()
        {
            var isVisibleCondition = ShowTypeOfItems == TypeOfItems.VisibleItems ? true : ShowTypeOfItems == TypeOfItems.HiddenItems ? false : (bool?)null;

            var filteredSteamItensInfo = _allSteamItensInformation
                .Where(i => !isVisibleCondition.HasValue || i.IsVisible == isVisibleCondition)
                .ToList();

            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>(filteredSteamItensInfo);
        }

        public enum TypeOfItems
        {
            VisibleItems,
            HiddenItems
        }
    }
}
