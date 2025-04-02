using SteamInventoryBoosterV2.Views;
using System.Windows.Controls;

namespace SteamInventoryBoosterV2.ViewModels
{
    public class MainAppWindowViewModel : BaseViewModel
    {
        private readonly SteamItemsPage _steamItemsPage;
        private readonly InventoryManagementPage _inventoryManagementPage;
        private readonly BotManagerPage _botManagerPage;
        private readonly SettingsPage _settingsPage;

        public MainAppWindowViewModel(SteamItemsPage steamItemsPage, InventoryManagementPage inventoryManagementPage, BotManagerPage botManagerPage, SettingsPage settingsPage)
        {
            _steamItemsPage = steamItemsPage ?? throw new ArgumentNullException(nameof(steamItemsPage));
            _inventoryManagementPage = inventoryManagementPage ?? throw new ArgumentNullException(nameof(inventoryManagementPage));
            _botManagerPage = botManagerPage ?? throw new ArgumentNullException(nameof(botManagerPage));
            _settingsPage = settingsPage ?? throw new ArgumentNullException(nameof(settingsPage));

            CurrentPage = _steamItemsPage;
        }

        private object? _currentPage;
        public object? CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        private ListBoxItem? _selectedNavItem;
        public ListBoxItem? SelectedNavItem
        {
            get => _selectedNavItem;

            set
            {
                if (_selectedNavItem != value)
                {
                    _selectedNavItem = value;
                    OnPropertyChanged(nameof(SelectedNavItem));
                    Navigate(_selectedNavItem?.Tag.ToString());
                }
            }
        }

        private void Navigate(string? pageKey)
        {
            if (string.IsNullOrEmpty(pageKey))
            {
                throw new ArgumentNullException(nameof(pageKey), "The value of pageKey cannot be null or empty.");
            }

            switch (pageKey)
            {
                case "SteamItemsPage":
                    CurrentPage = _steamItemsPage;
                    break;
                case "InventoryManagementPage":
                    CurrentPage = _inventoryManagementPage;
                    break;
                case "BotManagerPage":
                    CurrentPage = _botManagerPage;
                    break;
                case "SettingsPage":
                    CurrentPage = _settingsPage;
                    break;
                default:
                    throw new ArgumentException("The specified page is not valid.", nameof(pageKey));
            }
        }
    }
}
