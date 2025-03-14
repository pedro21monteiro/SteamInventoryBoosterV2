using SteamInventoryBoosterV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
