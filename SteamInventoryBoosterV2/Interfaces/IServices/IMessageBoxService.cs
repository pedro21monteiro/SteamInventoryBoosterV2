using System.Windows;

namespace SteamInventoryBoosterV2.Interfaces.IServices
{
    public interface IMessageBoxService
    {
        MessageBoxResult ShowMessage(string message, string title = "Information");
        MessageBoxResult ShowYesNo(string message, string title = "Confirmation");
        MessageBoxResult ShowError(string message, string title = "Error");
        MessageBoxResult ShowWarning(string message, string title = "Warning");
        MessageBoxResult ShowInfo(string message, string title = "Information");
        MessageBoxResult ShowSuccess(string message, string title = "Success");
    }
}
