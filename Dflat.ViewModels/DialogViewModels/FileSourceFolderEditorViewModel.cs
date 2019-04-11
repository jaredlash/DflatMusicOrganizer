using Dflat.Business;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;

namespace Dflat.ViewModels.DialogViewModels
{
    public class FileSourceFolderEditorViewModel : DialogViewModelBase
    {
        private IUnitOfWorkLifetimeManager uowLifetimeManager;
        private FileSourceFolder fileSourceFolder;
        private bool includeInScans;

        private string path;
        
        public FileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, FileSourceFolderEditorMode mode) : base("", "")
        {
            this.uowLifetimeManager = uowLifetimeManager;
            this.fileSourceFolder = fileSourceFolder;
            this.EditorMode = mode;

            // Default new FileSourceFolders to be included in scans
            if (fileSourceFolder.FileSourceFolderID == 0)
                includeInScans = true;

            ExcludePaths = new ObservableCollection<string>();

            CloseRequested += CloseRequestedHandler;
        }

        #region Public bindable properties

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public bool IncludeInScans
        {
            get { return includeInScans; }
            set
            {
                if (includeInScans != value)
                {
                    includeInScans = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICollection<string> ExcludePaths { get; private set; }

        #endregion


        public FileSourceFolderEditorMode EditorMode { get; private set; }

        #region Close event handler

        private void CloseRequestedHandler(object o, DialogCloseRequestedEventArgs a)
        {
            if (a.DialogResult == true)
            {
                // Update our FileSourceFolder
                throw new NotImplementedException("CloseRequestedHandler");
            }
        }

        #endregion
    }
}
