using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class ConfirmDialogViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }

        public ICommand YesCommand { get => new RelayCommand(() => Yes()); }
        public ICommand NoCommand { get => new RelayCommand(() => No()); }

        public void Yes()
        {
            TryClose(true);
        }

        public void No()
        {
            TryClose(false);
        }
    }
}
