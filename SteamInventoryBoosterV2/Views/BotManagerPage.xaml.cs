using SteamInventoryBoosterV2.ViewModels;
using System.Windows.Controls;

namespace SteamInventoryBoosterV2.Views
{
    /// <summary>
    /// Interaction logic for BotManagerPage.xaml
    /// </summary>
    public partial class BotManagerPage : Page
    {
        public BotManagerPage(BotManagerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
