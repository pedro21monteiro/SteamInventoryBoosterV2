using Microsoft.IdentityModel.Tokens;
using SteamInventoryBoosterV2.ViewModels.Commands;
using SteamInventoryBoosterV2.ViewModels.ItemViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static SteamInventoryBoosterV2.ViewModels.ItemViewModels.LogItemModel;
using static SteamInventoryBoosterV2.ViewModels.ItemViewModels.SteamItemsViewModel;

namespace SteamInventoryBoosterV2.ViewModels
{
    public class BotManagerViewModel : BaseViewModel
    {
        public BotManagerViewModel()
        {
            StartStopCommand = new RelayCommandAsync(StartOrStop, _ => CanStartOrStop);

            LogItems = new ObservableCollection<LogItemModel>();

            

            //LogItems.Add(new LogItemModel(DateTime.Now, "Login", "User1", LogStatus.Success, "Login bem-sucedido"));
            //LogItems.Add(new LogItemModel(DateTime.Now, "Logout", "User2", LogStatus.Failure, "Erro ao sair"));
            //LogItems.Add(new LogItemModel(DateTime.Now, "Update", "FileX", LogStatus.InProgress, "Atualização em andamento"));
        }

        // Commands
        public ICommand StartStopCommand { get; }
        public ICommand ClearCommand { get; }


        private ObservableCollection<LogItemModel> _logItems;
        public ObservableCollection<LogItemModel> LogItems
        {
            get => _logItems;
            set
            {
                _logItems = value;
                OnPropertyChanged(nameof(LogItems));
            }
        }

        private TypeOfFunctions? _selectedFunction;
        public TypeOfFunctions? SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                if (_selectedFunction != value)
                {
                    _selectedFunction = value;
                    OnPropertyChanged(nameof(SelectedFunction));
                    OnPropertyChanged(nameof(CanStartOrStop));
                    ((RelayCommandAsync)StartStopCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public enum TypeOfFunctions
        {
            SellItems,
            UpdatePrices
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(CanStartOrStop));
                OnPropertyChanged(nameof(IsComboBoxEnabled));
                OnPropertyChanged(nameof(StartStopButtonText));
                OnPropertyChanged(nameof(StartStopButtonColor));
            }
        }

        public bool CanStartOrStop => SelectedFunction.HasValue;
        public bool IsComboBoxEnabled => !IsRunning;
        public string StartStopButtonText => IsRunning ? "Stop" : "Start";
        public Brush StartStopButtonColor => IsRunning ? Brushes.Red : Brushes.Green;

        private async Task StartOrStop(object parameter)
        {
            if (IsRunning)
            {
                IsRunning = false; // Simula a interrupção do processo
                return;
            }

            IsRunning = true;
            await Task.Delay(5000); // Simula um processo longo
            IsRunning = false;
        }
    }
}
