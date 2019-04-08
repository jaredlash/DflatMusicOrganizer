using Dflat.Business;
using Dflat.Business.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase, IViewModel
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private IUnitOfWorkLifetimeManager uowManager;

        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowManager)
        {
            this.FileSourceFolders = new ObservableCollection<FileSourceFolder>();
            this.uowManager = uowManager;

            ((ObservableCollection<FileSourceFolder>)FileSourceFolders).CollectionChanged += FileSourceFolders_Changed;
        }

        #endregion

        #region ViewModel Load

        public void ViewModelInitialize()
        {

            LoadFileSourceFolders();
        }

        private void LoadFileSourceFolders()
        {
            FileSourceFolders.Clear();
            SelectedFileSourceFolder = null;

            foreach (var fileSourceFolder in uowManager.UnitOfWork.IFileSourceFolderRepository.GetAll())
                FileSourceFolders.Add(fileSourceFolder);

            // Add in some test file source folders
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"Z:\music\albums",
                IncludeInScans = true,
            });
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"C:\Users\Jared\Desktop",
                IncludeInScans = false,
            });
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"C:\Users\Jared\Some\Other\Really\Nested\Directory\With a possible long name\Somewhere",
                IncludeInScans = false,
            });
        }

        #endregion

        #region Override on close

        //public override void ViewModelClose()
        //{
        //    if (uowManager != null)
        //        uowManager.Dispose();
        //        
        //    base.ViewModelClose();
        //}

        #endregion

        #region Override Cleanup

        public override void Cleanup()
        {
            if (uowManager != null)
                uowManager.Dispose();

            base.Cleanup();
        }


        #endregion

        #region Event callbacks

        private void FileSourceFolders_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Count));
        }

        #endregion


        #region Commands

        public ICommand SaveCommand {
            get {
                return new RelayCommand(() => uowManager.UnitOfWork.SaveChanges(), () => uowManager.UnitOfWork.HasChanges());
            }
        }


        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() => ViewModelInitialize());
            }
        }


        #endregion

        #region Public Properties



        public ICommand ClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((e) => OnClosing(e));
            }
        }

        private void OnClosing(CancelEventArgs args)
        {
            args.Cancel = false;
        }


        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() => OnClose());
            }
        }
        
        public void OnClose()
        {
            Cleanup();
        }

        public ICollection<FileSourceFolder> FileSourceFolders { get; private set; }

        public int Count {
            get { return FileSourceFolders.Count; }
        }

        public FileSourceFolder SelectedFileSourceFolder
        {
            get
            {
                return selectedFileSourceFolder;
            }
            set
            {
                selectedFileSourceFolder = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanEdit));
                RaisePropertyChanged(nameof(SelectedFileSourceFolderExcludeCount));
            }
        }

        public int SelectedFileSourceFolderExcludeCount
        {
            get
            {
                return (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);
            }
        }

        public bool CanEdit
        {
            get { return SelectedFileSourceFolder != null; }
        }

        #endregion
    }
}
