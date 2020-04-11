using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public class MainWindowViewModel : Screen
    {

        #region Private backing fields


        private readonly IWindowManager windowManager;
        private readonly FileSourceManagerViewModel fileSourceManagerViewModel;

        #endregion

        #region Constructor

        public MainWindowViewModel(IWindowManager windowManager, FileSourceManagerViewModel fileSourceManagerViewModel)
        {
            this.windowManager = windowManager;
            this.fileSourceManagerViewModel = fileSourceManagerViewModel;
        }

        #endregion

        #region public Commands

        public async Task OpenFileSourceManager()
        {
            await windowManager.ShowDialogAsync(fileSourceManagerViewModel, null, null);
        }

        public void OpenJobsView()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
