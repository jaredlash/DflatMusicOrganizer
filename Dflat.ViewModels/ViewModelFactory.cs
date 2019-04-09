using Dflat.Business;
using Dflat.Business.Factories;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace Dflat.ViewModels
{
    public class ViewModelFactory : IViewModelFactory
    {
        
        private readonly IUnityContainer iocContainer;

        public ViewModelFactory(IUnitOfWorkFactory unitOfWorkFactory, IUnityContainer iocContainer)
        {
            this.iocContainer = iocContainer;
        }

        public FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager)
        {
            return iocContainer.Resolve<FileSourceManagerViewModel>(new ParameterOverride("uowManager", uowLifetimeManager));
        }
        
    }
}
