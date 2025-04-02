using SteamInventoryBoosterV2.Interfaces.IServices;
using System.Windows;

namespace SteamInventoryBoosterV2.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult ShowMessage(string message, string title = "Information")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult ShowYesNo(string message, string title = "Confirmation")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public MessageBoxResult ShowError(string message, string title = "Error")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult ShowWarning(string message, string title = "Warning")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public MessageBoxResult ShowInfo(string message, string title = "Information")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult ShowSuccess(string message, string title = "Success")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
