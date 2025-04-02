using SteamInventoryBoosterV2.ViewModels;
using System.Windows;

namespace SteamInventoryBoosterV2.Views
{
    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        public MainAppWindowViewModel ViewModel { get; }
        public MainAppWindow(MainAppWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }
    }
}
