using Dflat.Business;
using Dflat.Business.Models;
using Dflat.ViewModels.Dialogs;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private string selectedExcludePath;
        
        public FileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, IDialogService dialogService, FileSourceFolderEditorMode mode) : base("", "")
        {
            this.uowLifetimeManager = uowLifetimeManager;
            this.fileSourceFolder = fileSourceFolder;
            this.dialogService = dialogService;
            this.EditorMode = mode;

            path = fileSourceFolder.Path;

            // Default new FileSourceFolders to be included in scans
            if (fileSourceFolder.FileSourceFolderID == 0)
                includeInScans = true;
            else
                includeInScans = fileSourceFolder.IncludeInScans;

            ExcludePaths = new ObservableCollection<string>();
            foreach (var excludePath in fileSourceFolder.ExcludePaths)
                ExcludePaths.Add(excludePath.Path);

            selectedExcludePath = null;

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
                    RaisePropertyChanged(nameof(CanConfirmDialog));
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

        public string SelectedExcludePath
        {
            get
            {
                return selectedExcludePath;
            }
            set
            {
                selectedExcludePath = value;
                RaisePropertyChanged();
                ((RelayCommand)RemoveExcludePathCommand).RaiseCanExecuteChanged();
            }
        }

        public ICollection<string> ExcludePaths { get; private set; }

        public bool CanConfirmDialog
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Path);
            }
        }

        #endregion

        #region Commands

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
                return new RelayCommand(() => RemoveExcludePath(), () => selectedExcludePath != null);
            }
        }
        
        #endregion


        public FileSourceFolderEditorMode EditorMode { get; private set; }

        #region Private methods and event handlers

        private void ChoosePath()
        {
            string newPath = dialogService.FolderChooserDialog("Choose File Source Folder", path);

            if (!string.IsNullOrEmpty(newPath))
            {
                Path = newPath;
            }
        }

        private void AddExcludePath()
        {
            string newPath = dialogService.FolderChooserDialog("Choose Folder to exclude", path);

            if (string.IsNullOrEmpty(newPath))
                return;

            // Only add exclude folder if we don't already have it
            if (!ExcludePaths.Contains(newPath))
                ExcludePaths.Add(newPath);

        }

        private void RemoveExcludePath()
        {
            if (selectedExcludePath == null)
                return;

            ExcludePaths.Remove(SelectedExcludePath);
        }


        private void CloseRequestedHandler(object o, DialogCloseRequestedEventArgs a)
        {
            if (a.DialogResult != true)
                return;

            fileSourceFolder.Path = path;
            fileSourceFolder.IncludeInScans = IncludeInScans;

            // Determine which ExcludePaths need to be removed from the fileSourceFolder
            var pathsToRemove = new List<ExcludePath>();
            foreach (var excludePath in fileSourceFolder.ExcludePaths)
            {
                if (!ExcludePaths.Contains(excludePath.Path))
                    pathsToRemove.Add(excludePath);
            }

            // Remove the paths
            foreach (var excludePath in pathsToRemove)
                fileSourceFolder.ExcludePaths.Remove(excludePath);

            // Add the new paths
            foreach (var newExcludePath in ExcludePaths)
                if (fileSourceFolder.ExcludePaths.Where(e => e.Path == newExcludePath).Count() == 0)
                    fileSourceFolder.ExcludePaths.Add(new ExcludePath { Path = newExcludePath });
            
        }

        #endregion
    }
}
