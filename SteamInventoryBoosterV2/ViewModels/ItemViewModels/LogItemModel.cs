using System.ComponentModel;

namespace SteamInventoryBoosterV2.ViewModels.ItemViewModels
{
    public class LogItemModel : INotifyPropertyChanged
    {
        public LogItemModel(DateTime timestamp, string action, string itemName, LogStatus status, string message)
        {
            Timestamp = timestamp;
            Action = action;
            ItemName = itemName;
            Status = status;
            Message = message;
        }

        public LogItemModel(string action, string itemName, LogStatus status, string message)
            : this(DateTime.Now, action, itemName, status, message)
        {
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }

        private string _action = String.Empty;
        public string Action
        {
            get => _action;
            set
            {
                _action = value;
                OnPropertyChanged(nameof(Action));
            }
        }

        private string _itemName = String.Empty;
        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private LogStatus _status;
        public LogStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string _message = String.Empty;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public enum LogStatus
        {
            Started,
            Success,
            Failure,
            Pending,
            InProgress,
            Cancelled,
            Stopped,
            Finished
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
