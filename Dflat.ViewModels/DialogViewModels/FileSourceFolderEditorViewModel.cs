using Dflat.Business;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.ViewModels.DialogViewModels
{
    public class FileSourceFolderEditorViewModel : DialogViewModelBase
    {
        private IUnitOfWorkLifetimeManager uowLifetimeManager;
        private FileSourceFolder fileSourceFolder;

        public FileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder) : base("", "")
        {
            this.uowLifetimeManager = uowLifetimeManager;
            this.fileSourceFolder = fileSourceFolder;
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


    }
}
