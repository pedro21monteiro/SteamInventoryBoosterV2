using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class SteamItemsViewModel : BaseViewModel
    {

        public SteamItemsViewModel()
        {
            FilteredSteamItensInformation = new ObservableCollection<SteamItemModel>();

            FilteredSteamItensInformation.Add(new SteamItemModel { Name = "Item 1", BuyPrice = 2.5, SellPrice = 3.0, LastVerification = DateTime.Now, AtualPrice = 3.2, HighestPurchaseOrder = 2.8, IconPath = "/Assets/AppImage.png" });
        }

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

        private bool _isSearchResultVisible;
        public bool IsSearchResultVisible
        {
            get => _isSearchResultVisible;
            set
            {
                _isSearchResultVisible = value;
                OnPropertyChanged(nameof(IsSearchResultVisible));
            }
        }
    }
}
