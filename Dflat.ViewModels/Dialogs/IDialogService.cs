using Dflat.Business;
using Dflat.Business.Models;

namespace Dflat.ViewModels.Dialogs
{
    public interface IDialogService
    {
        bool? ConfirmDialog(string title, string message, string confirmButtonText, string denyButtonText);

        bool? FileSourceFolderEditor(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder);
        
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : DialogViewModelBase;
    }
}