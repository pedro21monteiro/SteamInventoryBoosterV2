using SteamInventoryBoosterV2.Services;
using System.ComponentModel;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class SteamItemModel : INotifyPropertyChanged
    {
        private string _classId = String.Empty;
        public string ClassId
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged(nameof(ClassId));
            }
        }

        private string _link = String.Empty;
        public string Link
        {
            get => _link;
            set
            {
                _link = value;
                OnPropertyChanged(nameof(Link));
            }
        }

        private string _name = String.Empty;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private decimal _buyPrice;
        public decimal BuyPrice
        {
            get => _buyPrice;
            set
            {
                _buyPrice = value;
                OnPropertyChanged(nameof(BuyPrice));
                OnPropertyChanged(nameof(SellProfit));
                OnPropertyChanged(nameof(ProfitPercentage));
            }
        }

        private decimal _sellPrice;
        public decimal SellPrice
        {
            get => _sellPrice;
            set
            {
                _sellPrice = value;
                OnPropertyChanged(nameof(SellPrice));
                OnPropertyChanged(nameof(SellProfit));
                OnPropertyChanged(nameof(ProfitPercentage));
            }
        }

        public decimal SellProfit => Calculus.SellItemProfit(BuyPrice, SellPrice);

        public decimal ProfitPercentage => Calculus.SellItemProfitPercentage(BuyPrice, SellPrice);

        private DateTime _lastVerification;
        public DateTime LastVerification
        {
            get => _lastVerification;
            set
            {
                _lastVerification = value;
                OnPropertyChanged(nameof(LastVerification));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private decimal _highestPurchaseOrder;
        public decimal HighestPurchaseOrder
        {
            get => _highestPurchaseOrder;
            set
            {
                _highestPurchaseOrder = value;
                OnPropertyChanged(nameof(HighestPurchaseOrder));
            }
        }
        private string _iconUrl = String.Empty;
        public string IconUrl
        {
            get => _iconUrl;
            set
            {
                _iconUrl = value;
                OnPropertyChanged(nameof(IconUrl));
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
