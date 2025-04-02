using SteamInventoryBoosterV2.ViewModels.ItemViewModels;
using System.Windows.Controls;

namespace SteamInventoryBoosterV2.Views
{
    /// <summary>
    /// Interaction logic for SteamItemsPage.xaml
    /// </summary>
    public partial class SteamItemsPage : Page
    {
        public SteamItemsPage(SteamItemsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
