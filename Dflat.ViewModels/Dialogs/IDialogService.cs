using Dflat.Business;
using Dflat.Business.Models;
using Dflat.ViewModels.DialogViewModels;

namespace Dflat.ViewModels.Dialogs
{
    public interface IDialogService
    {
        bool? ConfirmDialog(string title, string message, string confirmButtonText, string denyButtonText);

        string FolderChooserDialog(string title, string initialFolder);

        bool? FileSourceFolderEditor(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, FileSourceFolderEditorMode mode);
        
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : DialogViewModelBase;
    }
}