using Dflat.Business;
using Dflat.Business.Models;
using Dflat.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dflat.ViewModels.DialogViewModels
{
    public class FileSourceFolderEditorViewModel : DialogViewModelBase
    {
        private IUnitOfWorkLifetimeManager uowLifetimeManager;
        private FileSourceFolder fileSourceFolder;
        private IDialogService dialogService;

        private bool includeInScans;

        private string path;
        private int selectedExcludePathIndex;
        
        public FileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, IDialogService dialogService, FileSourceFolderEditorMode mode) : base("", "")
        {
            this.uowLifetimeManager = uowLifetimeManager;
            this.fileSourceFolder = fileSourceFolder;
            this.dialogService = dialogService;
            this.EditorMode = mode;

            // Default new FileSourceFolders to be included in scans
            if (fileSourceFolder.FileSourceFolderID == 0)
                includeInScans = true;

            ExcludePaths = new ObservableCollection<string>();

            selectedExcludePathIndex = -1;

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

        public int SelectedExcludePathIndex
        {
            get
            {
                return selectedExcludePathIndex;
            }
            set
            {
                selectedExcludePathIndex = value;
                RaisePropertyChanged();
                ((RelayCommand)RemoveExcludePathCommand).RaiseCanExecuteChanged();
            }
        }

        public ICollection<string> ExcludePaths { get; private set; }

        #endregion

        #region

        public ICommand ChoosePathCommand
        {
            get
            {
                return new RelayCommand(() => ChoosePath());
            }
        }

        public ICommand AddExcludePathCommand
        {
            get
            {
                return new RelayCommand(() => AddExcludePath());
            }
        }

        public ICommand RemoveExcludePathCommand
        {
            get
            {
                return new RelayCommand(() => RemoveExcludePath(), () => selectedExcludePathIndex >= 0);
            }
        }
        
        #endregion


        public FileSourceFolderEditorMode EditorMode { get; private set; }

        #region Private methods and event handlers

        private void ChoosePath()
        {

        }

        private void AddExcludePath()
        {

        }

        private void RemoveExcludePath()
        {

        }


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
