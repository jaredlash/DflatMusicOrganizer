namespace Dflat.ViewModels.Dialogs
{
    public interface IDialogService
    {
        bool? ConfirmDialog(string title, string message, string confirmButtonText, string denyButtonText);
        
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : DialogViewModelBase;
    }
}