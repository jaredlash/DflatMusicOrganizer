using AutoMapper;
using Caliburn.Micro;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceManagerViewModel : Screen
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private bool enableOverlay;
        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IJobService<FileSourceFolderScanJob> fileSourceFolderScanService;
        private readonly IMapper mapper;
        private readonly IWindowManager windowManager;
        private readonly ConfirmDialogViewModel confirmDialogViewModel;
        private readonly AlertDialogViewModel alertDialogViewModel;
        private readonly FileSourceFolderEditorViewModel sourceFolderEditorViewModel;

        private readonly List<FileSourceFolder> removedFolders;

        private readonly HashSet<FileSourceFolder> addedOrUpdatedFolders;
        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IFileSourceFolderRepository fileSourceFolderRepository,
                                          IJobService<FileSourceFolderScanJob> fileSourceFolderScanService,
                                          IMapper mapper,
                                          IWindowManager windowManager,
                                          ConfirmDialogViewModel confirmDialogViewModel,
                                          AlertDialogViewModel alertDialogViewModel,
                                          FileSourceFolderEditorViewModel sourceFolderEditorViewModel)
        {
            this.FileSourceFolders = new BindingList<FileSourceFolder>();
            this.removedFolders = new List<FileSourceFolder>();
            this.addedOrUpdatedFolders = new HashSet<FileSourceFolder>();
            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileSourceFolderScanService = fileSourceFolderScanService;
            this.mapper = mapper;
            this.windowManager = windowManager;
            this.confirmDialogViewModel = confirmDialogViewModel;
            this.alertDialogViewModel = alertDialogViewModel;
            this.sourceFolderEditorViewModel = sourceFolderEditorViewModel;

            this.EnableOverlay = false;
        }

        #endregion

        #region ViewModel Load

        protected override void OnViewLoaded(object view)
        {
            LoadFileSourceFolders();
            this.EnableOverlay = false;
        }



        private void LoadFileSourceFolders()
        {
            FileSourceFolders.Clear();
            removedFolders.Clear();
            SelectedFileSourceFolder = null;

            IEnumerable<FileSourceFolder> sources = new List<FileSourceFolder>();

            try
            {

                sources = fileSourceFolderRepository.GetAll();
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem loading source folders"; ;
                alertDialogViewModel.Message = $"A problem was encountered when loading the source folders: {ex.Message}";
                windowManager.ShowDialogAsync(alertDialogViewModel, null, null);
            }


            foreach (var fileSourceFolder in sources)
                FileSourceFolders.Add(fileSourceFolder);
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
                NotifyOfPropertyChange(() => CanRemove);
                NotifyOfPropertyChange(() => SelectedFileSourceFolderExcludeCount);
            }
        }

        public int SelectedFileSourceFolderExcludeCount => (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);

        public async Task Add()
        {
            var currentFolder = new FileSourceFolder();

            mapper.Map<FileSourceFolder, FileSourceFolderEditorViewModel>(currentFolder, sourceFolderEditorViewModel);
            sourceFolderEditorViewModel.IsChanged = currentFolder.IsChanged;
            bool? acceptChanges = await windowManager.ShowDialogAsync(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                mapper.Map<FileSourceFolderEditorViewModel, FileSourceFolder>(sourceFolderEditorViewModel, currentFolder);
                FileSourceFolders.Add(currentFolder);
            }
            NotifyOfPropertyChange(() => HasChanges);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CancelButtonText);
        }

        public bool CanEdit => SelectedFileSourceFolder != null;

        public async Task Edit()
        {
            var currentFolder = SelectedFileSourceFolder;

            mapper.Map<FileSourceFolder, FileSourceFolderEditorViewModel>(currentFolder, sourceFolderEditorViewModel);
            sourceFolderEditorViewModel.IsChanged = currentFolder.IsChanged;
            bool? acceptChanges = await windowManager.ShowDialogAsync(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                mapper.Map<FileSourceFolderEditorViewModel, FileSourceFolder>(sourceFolderEditorViewModel, currentFolder);
            }
            NotifyOfPropertyChange(() => HasChanges);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CancelButtonText);
        }

        public bool CanRemove => SelectedFileSourceFolder != null;

        public void Remove()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            if (currentFolder.FileSourceFolderID != 0) removedFolders.Add(currentFolder);

            FileSourceFolders.Remove(currentFolder);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CancelButtonText);
            NotifyOfPropertyChange(() => HasChanges);
        }

        public bool EnableOverlay
        {
            get { return enableOverlay; }
            set
            {
                enableOverlay = value;
                NotifyOfPropertyChange(() => EnableOverlay);
            }
        }

        public bool CanSave => HasChanges;

        public async Task Save()
        {
            EnableOverlay = true;
            try
            {
                foreach (var folder in FileSourceFolders)
                {
                    if (folder.IsChanged)
                        addedOrUpdatedFolders.Add(folder);
                }
                await fileSourceFolderRepository.UpdateAllAsync(FileSourceFolders);
                removedFolders.Clear();
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem saving changes"; ;
                alertDialogViewModel.Message = $"Problem saving changes: {ex.Message}";
                _ = await windowManager.ShowDialogAsync(alertDialogViewModel, null, null);
            }
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CancelButtonText);

            EnableOverlay = false;
        }

      
        public string CancelButtonText
        {
            get { return HasChanges ? "Cancel" : "Close"; }
        }


        public async Task Cancel()
        {
            await TryCloseAsync();
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (HasChanges)
            {
                if (await PromptUnsavedChanges() != true)
                    return false;
            }

            if (addedOrUpdatedFolders.Count > 0)
            {
                if (await PromptToScanFolders() == true)
                {
                    foreach (var folder in addedOrUpdatedFolders)
                    {
                        fileSourceFolderScanService.SubmitJobRequest(
                            new FileSourceFolderScanJob
                            {
                                FileSourceFolderID = folder.FileSourceFolderID,
                                Description = $"Scan of '{folder.Name}' {folder.Path}"
                            });
                    }
                }
            }
            return true;
        }

        private async Task<bool?> PromptUnsavedChanges()
        {
            confirmDialogViewModel.Title = "Discard unsaved Changes?";
            confirmDialogViewModel.Message = "There are unsaved changes. Click Cancel to go back and Save.";
            confirmDialogViewModel.NoText = "Cancel";
            confirmDialogViewModel.YesText = "Close";
            var result = await windowManager.ShowDialogAsync(confirmDialogViewModel, null, null);
            return result;
        }


        private async Task<bool?> PromptToScanFolders()
        {
            confirmDialogViewModel.Title = "Scan updated folders?";
            confirmDialogViewModel.Message = "File Source folders have been changed.  Would you like to scan the changed folders?";
            confirmDialogViewModel.NoText = "No";
            confirmDialogViewModel.YesText = "Yes";
            var result = await windowManager.ShowDialogAsync(confirmDialogViewModel, null, null);
            return result;
        }
        #endregion


        #region Private methods

        private bool HasChanges => removedFolders.Count > 0 || FileSourceFolders.Any(f => f.IsChanged);


        #endregion
    }
}

