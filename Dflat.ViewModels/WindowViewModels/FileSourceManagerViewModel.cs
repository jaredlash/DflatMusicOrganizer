using Dflat.Business;
using Dflat.Business.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Dflat.ViewModels.Dialogs;

namespace Dflat.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase, IViewModel
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private IUnitOfWorkLifetimeManager uowManager;
        private IViewService viewService;
        private IDialogService dialogService;
        private IViewModelFactory viewModelFactory;

        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowManager, IViewService viewService, IDialogService dialogService, IViewModelFactory viewModelFactory)
        {
            this.FileSourceFolders = new ObservableCollection<FileSourceFolder>();
            this.uowManager = uowManager;
            this.viewService = viewService;
            this.dialogService = dialogService;
            this.viewModelFactory = viewModelFactory;

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

        public ICommand AddCommand
        {
            get
            {
                return new RelayCommand(() => AddFileSourceFolder());
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(() => EditFileSourceFolder());
            }
        }

        public ICommand SaveCommand {
            get {
                return new RelayCommand(() => SaveChanges());
            }
        }


        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() => ViewModelInitialize());
            }
        }
        
        public ICommand ClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((e) => OnClosing(e));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() => OnClose());
            }
        }

        public ICommand RequestClose
        {
            get
            {
                return new RelayCommand<IView>((v) => CloseWindow(v));
            }
        }

        #endregion

        #region Bindable Public Properties



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


        #region Private methods

        private void AddFileSourceFolder()
        {
            FileSourceFolder newFileSourceFolder = uowManager.UnitOfWork.IFileSourceFolderRepository.Create();
            bool? result = dialogService.FileSourceFolderEditor(uowManager, newFileSourceFolder);
        }

        private void EditFileSourceFolder()
        {
            bool? result = dialogService.FileSourceFolderEditor(uowManager, SelectedFileSourceFolder);
        }
        
        private void OnClosing(CancelEventArgs args)
        {
            if (!uowManager.UnitOfWork.HasChanges())
                return;

            bool? result = dialogService.ConfirmDialog("Confirm Unsaved Changes", "There are unsaved changes. Click Cancel to Save", "Close", "Cancel");

            if (result == true)
                args.Cancel = false;
            else
                args.Cancel = true;
        }


        private void OnClose()
        {
            Cleanup();
        }

        private void CloseWindow(IView view)
        {
            view.Close();
        }

        private void SaveChanges()
        {
            if (uowManager.UnitOfWork.HasChanges())
                uowManager.UnitOfWork.SaveChanges();
        }
        #endregion
    }
}
