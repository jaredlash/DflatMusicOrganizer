using Dflat.Business;
using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public interface IViewModelFactory
    {

        FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager);

        ConfirmDialogViewModel CreateConfirmDialogViewModel(string title, string message, string confirmButtonText, string denyButtonText);
    }
}