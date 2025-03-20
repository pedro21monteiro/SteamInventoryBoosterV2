using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class SteamItemModel : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
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
            }
        }

        private decimal _sellProfit;
        public decimal SellProfit
        {
            get => _sellProfit;
            set
            {
                _sellProfit = value;
                OnPropertyChanged(nameof(SellProfit));
            }
        }

        private decimal _profitPercentage;
        public decimal ProfitPercentage
        {
            get => _profitPercentage;
            set
            {
                _profitPercentage = value;
                OnPropertyChanged(nameof(ProfitPercentage));
            }
        }

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

        private decimal _atualPrice;
        public decimal AtualPrice
        {
            get => _atualPrice;
            set
            {
                _atualPrice = value;
                OnPropertyChanged(nameof(AtualPrice));
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
        private string _iconPath = String.Empty;
        public string IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                OnPropertyChanged(nameof(IconPath));
                OnPropertyChanged(nameof(ItemIcon));
            }
        }

        public BitmapImage ItemIcon => new BitmapImage(new Uri(IconPath, UriKind.RelativeOrAbsolute));


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
