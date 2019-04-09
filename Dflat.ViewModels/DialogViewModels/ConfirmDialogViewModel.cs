using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public class ConfirmDialogViewModel : DialogViewModelBase
    {
        public string ConfirmButtonText;
        public string DenyButtonText;

        public ConfirmDialogViewModel(string title, string message, string confirmButtonText, string denyButtonText) : base(title, message)
        {
            ConfirmButtonText = confirmButtonText;
            DenyButtonText = denyButtonText;
        }


    }
}