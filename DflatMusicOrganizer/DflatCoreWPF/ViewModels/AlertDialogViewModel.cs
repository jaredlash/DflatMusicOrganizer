using CommunityToolkit.Mvvm.Input;

namespace DflatCoreWPF.ViewModels
{
    public partial class AlertDialogViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
        
        [RelayCommand]
        public void Okay()
        {
            TryClose(true);
        }
    }
}
