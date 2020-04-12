using AutoMapper;
using Caliburn.Micro;
using Dflat.Application.Models;
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
        private readonly IMapper mapper;
        private readonly IWindowManager windowManager;
        private readonly ConfirmDialogViewModel confirmDialogViewModel;
        private readonly FileSourceFolderEditorViewModel sourceFolderEditorViewModel;

        private readonly List<FileSourceFolder> removedFolders;

        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IMapper mapper,
                                          IWindowManager windowManager,
                                          ConfirmDialogViewModel confirmDialogViewModel,
                                          FileSourceFolderEditorViewModel sourceFolderEditorViewModel)
        {
            this.FileSourceFolders = new BindingList<FileSourceFolder>();
            this.removedFolders = new List<FileSourceFolder>();
            this.mapper = mapper;
            this.windowManager = windowManager;
            this.confirmDialogViewModel = confirmDialogViewModel;
            this.sourceFolderEditorViewModel = sourceFolderEditorViewModel;
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
            removedFolders.Clear();
            SelectedFileSourceFolder = null;

            //foreach (var fileSourceFolder in uowManager.UnitOfWork.FileSourceFolderRepository.GetAll())
            //    FileSourceFolders.Add(fileSourceFolder);

            //var testFolders = new List<FileSourceFolder>();
            //var temp1 = new FileSourceFolder
            //{
            //    FileSourceFolderID = 1,
            //    Name = "Mugsystem",
            //    Path = "C:\\"
            //};
            //temp1.IsChanged = false;
            //testFolders.Add(temp1);

            //var withExcludeFolders = new FileSourceFolder
            //{
            //    FileSourceFolderID = 2,
            //    Name = "Muganawa Media",
            //    Path = @"Z:\"
            //};
            //var ex1 = new ExcludePath { ExcludePathID = 1, FileSourceFolderID = 2, Path = @"Z:\video" };
            //var ex2 = new ExcludePath { ExcludePathID = 2, FileSourceFolderID = 2, Path = @"Z:\from_tim" };
            //var ex3 = new ExcludePath { ExcludePathID = 3, FileSourceFolderID = 2, Path = @"Z:\downloads" };
            //withExcludeFolders.ExcludePaths.Add(ex1);
            //withExcludeFolders.ExcludePaths.Add(ex2);
            //withExcludeFolders.ExcludePaths.Add(ex3);
            //withExcludeFolders.IsChanged = false;
            //testFolders.Add(withExcludeFolders);

            //testFolders.ForEach(f => FileSourceFolders.Add(f));
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

        public bool CanSave => HasChanges;

        public void Save()
        {

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
            confirmDialogViewModel.Title = "Discard unsaved Changes?";
            confirmDialogViewModel.Message = "There are unsaved changes. Click Cancel to go back and Save.";
            confirmDialogViewModel.NoText = "Cancel";
            confirmDialogViewModel.YesText = "Close";
            var result = await windowManager.ShowDialogAsync(confirmDialogViewModel, null, null);
            return result;
        }


        #endregion


        #region Private methods

        private bool HasChanges => removedFolders.Count > 0 || FileSourceFolders.Any(f => f.IsChanged);


        #endregion
    }
}

