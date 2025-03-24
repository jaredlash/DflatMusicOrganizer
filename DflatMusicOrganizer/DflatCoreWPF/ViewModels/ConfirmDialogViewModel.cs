using CommunityToolkit.Mvvm.Input;

namespace DflatCoreWPF.ViewModels
{
    public partial class ConfirmDialogViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }

        [RelayCommand]
        public void Yes()
        {
            TryClose(true);
        }

        [RelayCommand]
        public void No()
        {
            TryClose(false);
        }
    }
}
