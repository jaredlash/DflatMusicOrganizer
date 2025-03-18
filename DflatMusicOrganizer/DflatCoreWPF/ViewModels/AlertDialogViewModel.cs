using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class AlertDialogViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
        
        public ICommand OkayCommand { get => new RelayCommand(() => Okay()); }

        public void Okay()
        {
            TryClose(true);
        }
    }
}
