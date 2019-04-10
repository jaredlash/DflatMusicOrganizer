using Dflat.Business;
using Dflat.Business.Models;
using Dflat.ViewModels.DialogViewModels;

namespace Dflat.ViewModels
{
    public interface IViewModelFactory
    {

        FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager);
        FileSourceFolderEditorViewModel CreateFileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, FileSourceFolderEditorMode mode);

        ConfirmDialogViewModel CreateConfirmDialogViewModel(string title, string message, string confirmButtonText, string denyButtonText);
        
    }
}