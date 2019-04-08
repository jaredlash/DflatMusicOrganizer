using Dflat.Business.Factories;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    public class MainWindowViewModel : GalaSoft.MvvmLight.ViewModelBase    //ViewModelBase
    {

        #region Private backing fields

        private readonly IViewService viewService;
        private readonly IViewModelFactory viewModelFactory;
        private readonly IUowLifetimeManagerFactory uowLifetimeManagerFactory;
        
        #endregion

        #region Constructor

        public MainWindowViewModel(IUowLifetimeManagerFactory uowLifetimeManagerFactory, IViewModelFactory viewModelFactory, IViewService viewService)
        {
            this.uowLifetimeManagerFactory = uowLifetimeManagerFactory;
            this.viewModelFactory = viewModelFactory;
            this.viewService = viewService;
        }

        #endregion

        #region public ICommands

        public ICommand OpenFileSourceManagerCommand {
            get
            {
                return new RelayCommand(() => OpenFileSourceManager());
            }
        }

        public ICommand OpenJobsViewCommand
        {
            get
            {
                return new RelayCommand(() => OpenJobsView());
            }
        }

        #endregion


        #region Private methods

        private void OpenFileSourceManager()
        {
            var uowManager = uowLifetimeManagerFactory.Create();
            var fsmvm = viewModelFactory.CreateFileSourceManagerViewModel(uowManager);

            viewService.ShowWindow<FileSourceManagerViewModel>(fsmvm);
        }

        private void OpenJobsView()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
