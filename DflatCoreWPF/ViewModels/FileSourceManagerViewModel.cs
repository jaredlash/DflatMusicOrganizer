using Caliburn.Micro;
using DflatCoreWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceManagerViewModel : Screen
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private readonly IWindowManager windowManager;
        private readonly ConfirmDialogViewModel confirmDialogViewModel;

        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IWindowManager windowManager, ConfirmDialogViewModel confirmDialogViewModel)
        {
            this.FileSourceFolders = new BindingList<FileSourceFolder>();
            this.windowManager = windowManager;
            this.confirmDialogViewModel = confirmDialogViewModel;
        }

        #endregion

        #region ViewModel Load

        protected override void OnViewLoaded(object view)
        {
            LoadFileSourceFolders();
        }


        private void LoadFileSourceFolders()
        {
            FileSourceFolders.Clear();
            SelectedFileSourceFolder = null;

            //foreach (var fileSourceFolder in uowManager.UnitOfWork.FileSourceFolderRepository.GetAll())
            //    FileSourceFolders.Add(fileSourceFolder);
        }

        #endregion


        #region Caliburn Micro bindings

        public BindingList<FileSourceFolder> FileSourceFolders { get; private set; }

        public int Count
        {
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
                NotifyOfPropertyChange(() => SelectedFileSourceFolder);
                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => SelectedFileSourceFolderExcludeCount);
            }
        }

        public int SelectedFileSourceFolderExcludeCount => (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);

        public bool CanEdit => SelectedFileSourceFolder != null;

        public void Edit()
        {

        }

        public bool CanRemove => SelectedFileSourceFolder != null;

        public void Remove()
        {

        }

        public bool CanSave => HasChanges;
        public void Save()
        {

        }


        public async Task Cancel()
        {
            await TryCloseAsync();
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (HasChanges)
            {
                if (await PromptUnsavedChanges() == true)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        private async Task<bool?> PromptUnsavedChanges()
        {
            confirmDialogViewModel.Title = "Confirm Unsaved Changes";
            confirmDialogViewModel.Message = "There are unsaved changes. Click Cancel to Save";
            confirmDialogViewModel.NoText = "Cancel";
            confirmDialogViewModel.YesText = "Close";
            var result = await windowManager.ShowDialogAsync(confirmDialogViewModel, null, null);
            return result;
        }


        #endregion


        #region Private methods

        //private bool HasChanges => FileSourceFolders.Any(f => f.IsChanged);
        private bool HasChanges => true;

        //private void AddFileSourceFolder()
        //{
        //    FileSourceFolder newFileSourceFolder = new FileSourceFolder();
        //    bool? result = dialogService.FileSourceFolderEditor(uowManager, newFileSourceFolder, FileSourceFolderEditorMode.New);
        //    if (result == true)
        //    {
        //        uowManager.UnitOfWork.FileSourceFolderRepository.Add(newFileSourceFolder);

        //        // Added a new folder, so notify that we can save.
        //        ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        //        FileSourceFolders.Add(newFileSourceFolder);
        //        RaiseNotificationEventsAfterFolderEdit();
        //    }
        //}

        //private void EditFileSourceFolder()
        //{
        //    dialogService.FileSourceFolderEditor(uowManager, SelectedFileSourceFolder, FileSourceFolderEditorMode.Edit);
        //    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        //    RaiseNotificationEventsAfterFolderEdit();
        //    RaisePropertyChanged(nameof(SelectedFileSourceFolder));
        //}

        //private void RemoveFileSourceFolder()
        //{
        //    bool? result = dialogService.ConfirmDialog("Confirm", $"Are you sure you want to remove {SelectedFileSourceFolder.Path}?", "Yes", "Cancel");
        //    if (result == true)
        //    {
        //        uowManager.UnitOfWork.FileSourceFolderRepository.Remove(SelectedFileSourceFolder);
        //        FileSourceFolders.Remove(SelectedFileSourceFolder);
        //        ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        //        RaiseNotificationEventsAfterFolderEdit();
        //    }
        //}

        //private void RaiseNotificationEventsAfterFolderEdit()
        //{
        //    RaisePropertyChanged(nameof(Count));


        //    if (SelectedFileSourceFolder == null)
        //        return;


        //    RaisePropertyChanged(nameof(SelectedFileSourceFolder));
        //    RaisePropertyChanged(nameof(SelectedFileSourceFolderExcludeCount));
        //}



        //private void SaveChanges()
        //{
        //    if (uowManager.UnitOfWork.HasChanges())
        //        uowManager.UnitOfWork.SaveChanges();
        //}
        #endregion
    }
}

