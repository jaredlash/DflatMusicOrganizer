using Dflat.Business;
using Dflat.Business.Models;

namespace Dflat.ViewModels.DialogViewModels
{
    public class FileSourceFolderEditorViewModel : DialogViewModelBase
    {
        private IUnitOfWorkLifetimeManager uowLifetimeManager;
        private FileSourceFolder fileSourceFolder;
        
        public FileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, FileSourceFolderEditorMode mode) : base("", "")
        {
            this.uowLifetimeManager = uowLifetimeManager;
            this.fileSourceFolder = fileSourceFolder;
            this.EditorMode = mode;
        }
        
        public string Path
        {
            get
            {
                return fileSourceFolder.Path;
            }
            set
            {
                fileSourceFolder.Path = value;
                RaisePropertyChanged();
            }
        }

        public FileSourceFolderEditorMode EditorMode { get; private set; }


    }
}
